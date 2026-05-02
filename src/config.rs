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

#[cfg(test)]
mod tests {
    use super::*;

    const TOML: &str = r#"
    address = "localhost:6789"

    [hosts.localhost]
    "" = "https://localhost:1000/"
    "dashboard" = "https://localhost:2000/dev/dashboard"

    [hosts."admin.localhost"]
    "" = "https://localhost:3000/"
    "#;

    fn make_test_config() -> Config {
        toml::from_str::<Config>(TOML).unwrap_or_default()
    }

    #[test]
    fn default_config_address_not_empty() {
        let cfg = Config::default();
        assert!(!cfg.address.is_empty());
    }

    #[test]
    fn default_config_hosts_empty() {
        let cfg = Config::default();
        assert!(cfg.hosts.is_empty());
    }

    #[test]
    fn get_address_correct() {
        let cfg = make_test_config();
        assert_eq!(cfg.address, "localhost:6789");
    }

    #[test]
    fn get_target_invalid_invalid_none() {
        let cfg = make_test_config();
        assert!(cfg.get_target("invalid", "invalid").is_none());
    }

    #[test]
    fn get_target_valid_invalid_none() {
        let cfg = make_test_config();
        assert!(cfg.get_target("localhost", "invalid").is_none());
    }

    #[test]
    fn get_target_valid_valid_correct() {
        let cfg = make_test_config();
        assert_eq!(
            cfg.get_target("localhost", "dashboard"),
            Some("https://localhost:2000/dev/dashboard")
        );
    }

    #[test]
    fn get_target_valid_valid_empty_correct() {
        let cfg = make_test_config();
        assert_eq!(
            cfg.get_target("localhost", ""),
            Some("https://localhost:1000/")
        );
    }
}
