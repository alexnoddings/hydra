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
