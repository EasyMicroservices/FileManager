use anyhow::Result;
use async_trait::async_trait;

use crate::models::{DirectoryDetail, FileDetail};

pub trait PathProvider {
    fn combine(&self, paths: Vec<&str>) -> Result<String>;
    fn get_object_name(&self, path: &str) -> Result<String>;
    fn get_object_parent_path(&self, path: &str) -> Result<String>;
}

#[async_trait]
pub trait DirectoryManager: Send + Sync {
    fn path_provider(&self) -> &dyn PathProvider;
    async fn create_dir(&self, path: &str) -> Result<DirectoryDetail>;
    async fn get_dir(&self, path: &str) -> Result<DirectoryDetail>;
    async fn is_dir_exists(&self, path: &str) -> Result<bool>;
    async fn delete_dir(&self, path: &str, recursive: bool) -> Result<()>;
}

#[async_trait]
pub trait FileManager: Send + Sync {
    fn path_provider(&self) -> &dyn PathProvider;
    fn dir_manager(&self) -> &dyn DirectoryManager;
    async fn get_file(&self, path: &str) -> Result<FileDetail>;
    async fn create_file(&self, path: &str) -> Result<FileDetail>;
    async fn is_file_exists(&self, path: &str) -> Result<bool>;
    async fn delete_file(&self, path: &str) -> Result<()>;
}
