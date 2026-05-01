use std::sync::Arc;

mod config;
mod router;

#[tokio::main]
async fn main() {
    let cfg = config::load_config().await;
    let address = cfg.address.clone();
    let cfg = Arc::new(cfg);

    let listener = tokio::net::TcpListener::bind(&address)
        .await
        .expect("failed to bind to address");
    let router = router::build_router(cfg);

    println!("Now listening on: {address}");
    println!("Application started. Press Ctrl+C to shut down.");

    axum::serve(listener, router)
        .await
        .expect("Error running server");
}
