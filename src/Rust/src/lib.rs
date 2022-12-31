use anyhow::Result;

use providers::DirectoryManager;

use crate::providers::FileManager;

pub mod providers;
pub mod models;
pub mod disk;

pub async fn create_dir(path: &str, dir_manager: &dyn DirectoryManager) -> Result<bool> {
    dir_manager.create_dir(path).await
}

pub async fn get_dir(path: &str, dir_manager: &dyn DirectoryManager) -> Result<bool> {
    dir_manager.get_dir(path).await
}

pub async fn is_dir_exists(path: &str, dir_manager: &dyn DirectoryManager) -> Result<bool> {
    dir_manager.is_dir_exists(path).await
}

pub async fn delete_dir(path: &str, recursive: bool, dir_manager: &dyn DirectoryManager) -> Result<bool> {
    dir_manager.delete_dir(path, recursive).await
}

pub async fn get_file(path: &str, file_manager: &dyn FileManager) -> Result<bool> {
    file_manager.get_file(path).await
}

pub async fn create_file(path: &str, file_manager: &dyn FileManager) -> Result<bool> {
    file_manager.create_file(path).await
}

pub async fn is_file_exists(path: &str, file_manager: &dyn FileManager) -> Result<bool> {
    file_manager.is_file_exists(path).await
}

pub async fn delete_file(path: &str, file_manager: &dyn FileManager) -> Result<bool> {
    file_manager.delete_file(path).await
}
