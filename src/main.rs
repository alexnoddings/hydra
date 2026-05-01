mod config;

#[tokio::main]
async fn main() {
    let cfg = config::load_config().await;
    println!("{:#?}", cfg.address);
    println!("{:#?}", cfg.hosts);
}
