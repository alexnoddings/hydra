use crate::config::Config;
use axum::extract::{Path, State};
use axum::http::{HeaderMap, StatusCode};
use axum::response::{IntoResponse, Response};
use axum::routing::get;
use axum::Router;
use std::sync::Arc;

pub fn build_router(cfg: Arc<Config>) -> Router {
    Router::new()
        .route("/", get(handler))
        .route("/{*slug}", get(handler))
        .with_state(cfg)
}

async fn handler(
    State(cfg): State<Arc<Config>>,
    Path(slug): Path<String>,
    headers: HeaderMap,
) -> Response {
    let addr = &cfg.address;
    let Some(host) = headers.get("host").and_then(|h| h.to_str().ok()) else {
        return StatusCode::BAD_REQUEST.into_response();
    };

    format!("slug: {slug}, cfg bind: {addr}, host: {host}").into_response()
}
