use serde::Deserialize;
use std::collections::HashMap;

#[derive(Deserialize)]
pub struct Config {
    #[serde(default = "address_default")]
    pub address: String,

    #[serde(default)]
    pub hosts: HashMap<String, HashMap<String, String>>,
}

impl Config {
    pub fn get_target(&self, host: &str, path: &str) -> Option<&str> {
        self.hosts
            .get(host)
            .and_then(|h| h.get(path))
            .map(String::as_str)
    }
}

impl Default for Config {
    fn default() -> Self {
        Config {
            address: address_default(),
            hosts: HashMap::new(),
        }
    }
}

fn address_default() -> String {
    "localhost:8080".to_string()
}

pub async fn load_config() -> Config {
    let cfg_str = tokio::fs::read_to_string("hydra.toml").await;
    match cfg_str {
        Ok(cfg) => toml::from_str::<Config>(&cfg).unwrap_or_default(),
        Err(_) => Config::default(),
    }
}
