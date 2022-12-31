use anyhow::Result;
use async_trait::async_trait;

pub trait PathProvider {
    fn combine(&self, paths: Vec<&str>) -> Result<&str>;
    fn get_object_name(&self, path: &str) -> Result<&str>;
    fn get_object_parent_path(&self, path: &str) -> Result<&str>;
}

#[async_trait]
pub trait DirectoryManager {
    async fn create_dir(&self, path: &str) -> Result<bool>;
    async fn get_dir(&self, path: &str) -> Result<bool>;
    async fn is_dir_exists(&self, path: &str) -> Result<bool>;
    async fn delete_dir(&self, path: &str, recursive: bool) -> Result<bool>;
}

#[async_trait]
pub trait FileManager {
    async fn get_file(&self, path: &str) -> Result<bool>;
    async fn create_file(&self, path: &str) -> Result<bool>;
    async fn is_file_exists(&self, path: &str) -> Result<bool>;
    async fn delete_file(&self, path: &str) -> Result<bool>;
}
