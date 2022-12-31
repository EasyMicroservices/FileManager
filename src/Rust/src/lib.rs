use anyhow::Result;

use providers::DirectoryManager;

use crate::models::{DirectoryDetail, FileDetail};
use crate::providers::FileManager;

pub mod providers;
pub mod models;
pub mod disk;

pub async fn create_dir<'a>(path: &str, dir_manager: &'a dyn DirectoryManager) -> Result<DirectoryDetail<'a>> {
    dir_manager.create_dir(path).await
}

pub async fn get_dir<'a>(path: &str, dir_manager: &'a dyn DirectoryManager) -> Result<DirectoryDetail<'a>> {
    dir_manager.get_dir(path).await
}

pub async fn is_dir_exists(path: &str, dir_manager: &dyn DirectoryManager) -> Result<bool> {
    dir_manager.is_dir_exists(path).await
}

pub async fn delete_dir(path: &str, recursive: bool, dir_manager: &dyn DirectoryManager) -> Result<bool> {
    dir_manager.delete_dir(path, recursive).await
}

pub async fn get_file<'a>(path: &str, file_manager: &'a dyn FileManager) -> Result<FileDetail<'a>> {
    file_manager.get_file(path).await
}

pub async fn create_file<'a>(path: &str, file_manager: &'a dyn FileManager) -> Result<FileDetail<'a>> {
    file_manager.create_file(path).await
}

pub async fn is_file_exists(path: &str, file_manager: &dyn FileManager) -> Result<bool> {
    file_manager.is_file_exists(path).await
}

pub async fn delete_file(path: &str, file_manager: &dyn FileManager) -> Result<bool> {
    file_manager.delete_file(path).await
}
