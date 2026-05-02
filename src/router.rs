use crate::config::Config;
use axum::extract::{Path, State};
use axum::http::{header, HeaderMap, StatusCode};
use axum::response::{IntoResponse, Response};
use axum::routing::get;
use axum::Router;
use std::sync::Arc;

pub fn build_router(cfg: Arc<Config>) -> Router {
    Router::new()
        .route("/", get(handler_root))
        .route("/{*slug}", get(handler_slug))
        .with_state(cfg)
}

async fn handler_root(State(cfg): State<Arc<Config>>, headers: HeaderMap) -> Response {
    handler(cfg, headers, "").await
}

async fn handler_slug(
    State(cfg): State<Arc<Config>>,
    headers: HeaderMap,
    Path(slug): Path<String>,
) -> Response {
    handler(cfg, headers, &slug).await
}

async fn handler(cfg: Arc<Config>, headers: HeaderMap, path: &str) -> Response {
    let Some(host) = headers.get("host").and_then(|h| h.to_str().ok()) else {
        return StatusCode::BAD_REQUEST.into_response();
    };
    let host = trim_host(host);
    let path = trim_path(path);

    match cfg.get_target(host, path) {
        Some(target) => {
            let headers = [(header::LOCATION, target)];
            (StatusCode::FOUND, headers).into_response()
        }
        None => StatusCode::NOT_FOUND.into_response(),
    }
}

fn trim_host(host: &str) -> &str {
    // Lop the port off of the host, leaving the rest
    match host.rfind(':') {
        Some(i) => &host[..i],
        None => host,
    }
}

fn trim_path(path: &str) -> &str {
    // start is trimmed for us by the router
    path.trim_end_matches('/')
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::config::parse_config;
    use axum::body::Body;
    use axum::http::Request;
    use tower::ServiceExt;

    #[test]
    fn trim_host_no_port() {
        let host = "localhost";
        let trimmed = trim_host(host);
        assert_eq!(trimmed, "localhost");
    }

    #[test]
    fn trim_host_port() {
        let host = "localhost:8989";
        let trimmed = trim_host(host);
        assert_eq!(trimmed, "localhost");
    }

    #[test]
    fn trim_subdomain() {
        let host = "admin.localhost:8989";
        let trimmed = trim_host(host);
        assert_eq!(trimmed, "admin.localhost");
    }

    #[test]
    fn trim_path_trimmed() {
        let path = "dashboard";
        let trimmed = trim_path(path);
        assert_eq!(trimmed, "dashboard");
    }

    #[test]
    fn trim_path_untrimmed() {
        let path = "dashboard/";
        let trimmed = trim_path(path);
        assert_eq!(trimmed, "dashboard");
    }

    const TOML: &str = r#"
    address = "localhost:6789"

    [hosts.localhost]
    "" = "https://localhost:1000/"
    "dashboard" = "https://localhost:2000/dev/dashboard"

    [hosts."admin.localhost"]
    "" = "https://localhost:3000/"
    "#;

    fn make_test_config() -> Arc<Config> {
        let cfg = parse_config(TOML);
        Arc::new(cfg)
    }

    fn request(host: &str, path: &str) -> Request<Body> {
        let mut builder = Request::builder().uri(path);
        if !host.is_empty() {
            builder = builder.header(header::HOST, host);
        }
        builder.body(Body::empty()).unwrap()
    }

    #[tokio::test]
    async fn handler_no_host_bad_request() {
        let cfg = make_test_config();
        let router = build_router(cfg);
        let req = request("", "/");

        let resp = router.oneshot(req).await.unwrap();
        assert_eq!(resp.status(), StatusCode::BAD_REQUEST);
    }

    #[tokio::test]
    async fn handler_invalid_path_not_found() {
        let cfg = make_test_config();
        let router = build_router(cfg);
        let req = request("localhost", "/not-found");

        let resp = router.oneshot(req).await.unwrap();
        assert_eq!(resp.status(), StatusCode::NOT_FOUND);
    }

    #[tokio::test]
    async fn handler_path_found() {
        let cfg = make_test_config();
        let router = build_router(cfg);
        let req = request("localhost", "/");

        let resp = router.oneshot(req).await.unwrap();
        assert_eq!(resp.status(), StatusCode::FOUND);
        let location = resp.headers().get(header::LOCATION).unwrap();
        assert_eq!(location, "https://localhost:1000/");
    }

    #[tokio::test]
    async fn handler_path_untrimmed_found() {
        let cfg = make_test_config();
        let router = build_router(cfg);
        let req = request("localhost", "/dashboard/");

        let resp = router.oneshot(req).await.unwrap();
        assert_eq!(resp.status(), StatusCode::FOUND);
        let location = resp.headers().get(header::LOCATION).unwrap();
        assert_eq!(location, "https://localhost:2000/dev/dashboard");
    }

    #[tokio::test]
    async fn handler_path_trimmed_found() {
        let cfg = make_test_config();
        let router = build_router(cfg);
        let req = request("localhost", "/dashboard");

        let resp = router.oneshot(req).await.unwrap();
        assert_eq!(resp.status(), StatusCode::FOUND);
        let location = resp.headers().get(header::LOCATION).unwrap();
        assert_eq!(location, "https://localhost:2000/dev/dashboard");
    }
}
