use std::path;

use anyhow::{anyhow, bail};
use async_trait::async_trait;

use crate::{DirectoryManager, FileManager};
use crate::providers::PathProvider;

pub struct DiskFileManager {}

#[async_trait]
impl FileManager for DiskFileManager {
    fn path_provider(&self) -> &dyn PathProvider {
        todo!()
    }

    fn dir_manager(&self) -> &dyn DirectoryManager {
        todo!()
    }

    async fn get_file(&self, path: &str) -> anyhow::Result<bool> {
        todo!()
    }

    async fn create_file(&self, path: &str) -> anyhow::Result<bool> {
        todo!()
    }

    async fn is_file_exists(&self, path: &str) -> anyhow::Result<bool> {
        todo!()
    }

    async fn delete_file(&self, path: &str) -> anyhow::Result<bool> {
        todo!()
    }
}

pub struct DiskDirectoryManager {}

#[async_trait]
impl DirectoryManager for DiskDirectoryManager {
    fn path_provider(&self) -> &dyn PathProvider {
        todo!()
    }

    async fn create_dir(&self, path: &str) -> anyhow::Result<bool> {
        todo!()
    }

    async fn get_dir(&self, path: &str) -> anyhow::Result<bool> {
        todo!()
    }

    async fn is_dir_exists(&self, path: &str) -> anyhow::Result<bool> {
        todo!()
    }

    async fn delete_dir(&self, path: &str, recursive: bool) -> anyhow::Result<bool> {
        todo!()
    }
}

pub struct SystemPathProvider {}

impl PathProvider for SystemPathProvider {
    fn combine(&self, paths: Vec<&str>) -> anyhow::Result<String> {
        let mut buf = path::PathBuf::new();

        for path in paths {
            buf.push(path)
        }

        match buf.to_str() {
            Some(v) => Ok(v.to_string()),
            None => bail!("empty or invalid paths"),
        }
    }

    fn get_object_name(&self, path: &str) -> anyhow::Result<String> {
        let p = path::Path::new(path);

        if let Some(v) = p.file_name() {
            if let Some(v) = v.to_str() {
                return Ok(v.to_string());
            }
        }

        bail!("invalid path: {}", path)
    }

    fn get_object_parent_path(&self, path: &str) -> anyhow::Result<String> {
        let p = path::Path::new(path);

        if let Some(v) = p.parent() {
            if let Some(v) = v.to_str() {
                return Ok(v.to_string());
            }
        }

        bail!("invalid path: {}", path)
    }
}