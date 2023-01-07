pub mod providers;
pub mod models;
pub mod disk;

use anyhow::Result;

use crate::models::{DirectoryDetail, FileDetail};
use crate::providers::{FileManager, DirectoryManager};

pub async fn create_dir_async<'a>(path: &str, dir_manager: &'a dyn DirectoryManager) -> Result<DirectoryDetail<'a>> {
    dir_manager.create_dir_async(path).await
}

pub async fn get_dir_async<'a>(path: &str, dir_manager: &'a dyn DirectoryManager) -> Result<DirectoryDetail<'a>> {
    dir_manager.get_dir_async(path).await
}

pub async fn is_dir_exists_async(path: &str, dir_manager: &dyn DirectoryManager) -> Result<bool> {
    dir_manager.is_dir_exists_async(path).await
}

pub async fn delete_dir_async(path: &str, recursive: bool, dir_manager: &dyn DirectoryManager) -> Result<()> {
    dir_manager.delete_dir_async(path, recursive).await
}

pub async fn get_file_async<'a>(path: &str, file_manager: &'a dyn FileManager) -> Result<FileDetail<'a>> {
    file_manager.get_file_async(path).await
}

pub async fn create_file_async<'a>(path: &str, file_manager: &'a dyn FileManager) -> Result<FileDetail<'a>> {
    file_manager.create_file_async(path).await
}

pub async fn is_file_exists_async(path: &str, file_manager: &dyn FileManager) -> Result<bool> {
    file_manager.is_file_exists_async(path).await
}

pub async fn delete_file_async(path: &str, file_manager: &dyn FileManager) -> Result<()> {
    file_manager.delete_file_async(path).await
}
