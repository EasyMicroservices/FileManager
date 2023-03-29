use std::fmt::Debug;
use anyhow::Result;
use async_trait::async_trait;

use crate::models::{DirectoryDetail, FileDetail};

pub trait PathProvider: Send + Sync + Debug + helper::AsAny {
    fn combine(&self, paths: Vec<&str>) -> Result<String>;
    fn get_object_name(&self, path: &str) -> Result<String>;
    fn get_object_parent_path(&self, path: &str) -> Result<String>;
}

#[async_trait]
pub trait DirectoryManager: Send + Sync + Debug + helper::AsAny {
    fn path_provider(&self) -> &dyn PathProvider;
    // Sync methods
    async fn create_dir_async(&self, path: &str) -> Result<DirectoryDetail>;
    async fn get_dir_async(&self, path: &str) -> Result<DirectoryDetail>;
    async fn is_dir_exists_async(&self, path: &str) -> Result<bool>;
    async fn delete_dir_async(&self, path: &str, recursive: bool) -> Result<()>;
    // Async methods
    fn create_dir(&self, path: &str) -> Result<DirectoryDetail>;
    fn get_dir(&self, path: &str) -> Result<DirectoryDetail>;
    fn is_dir_exists(&self, path: &str) -> Result<bool>;
    fn delete_dir(&self, path: &str, recursive: bool) -> Result<()>;
}

#[async_trait]
pub trait FileManager: Send + Sync + Debug + helper::AsAny {
    fn path_provider(&self) -> &dyn PathProvider;
    fn dir_manager(&self) -> &dyn DirectoryManager;
    // Async methods
    async fn get_file_async(&self, path: &str) -> Result<FileDetail>;
    async fn create_file_async(&self, path: &str) -> Result<FileDetail>;
    async fn is_file_exists_async(&self, path: &str) -> Result<bool>;
    async fn delete_file_async(&self, path: &str) -> Result<()>;
    // Sync methods
    fn get_file(&self, path: &str) -> Result<FileDetail>;
    fn create_file(&self, path: &str) -> Result<FileDetail>;
    fn is_file_exists(&self, path: &str) -> Result<bool>;
    fn delete_file(&self, path: &str) -> Result<()>;
}


mod helper {
    use core::any::Any;

    pub trait AsAny: Any {
        fn as_any(&self) -> &dyn Any;
        fn as_any_mut(&mut self) -> &mut dyn Any;
    }

    impl<T: Any> AsAny for T {
        fn as_any(&self) -> &dyn Any {
            self
        }

        fn as_any_mut(&mut self) -> &mut dyn Any {
            self
        }
    }
}