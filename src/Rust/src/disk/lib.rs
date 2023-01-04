use std::path;
use anyhow::bail;
use async_trait::async_trait;
use tokio::fs;

use crate::models::{DirectoryDetail, FileDetail};
use crate::providers::{PathProvider, DirectoryManager, FileManager};

pub struct DiskFileManager {}

#[async_trait]
impl FileManager for DiskFileManager {
    fn path_provider(&self) -> &dyn PathProvider {
        todo!()
    }

    fn dir_manager(&self) -> &dyn DirectoryManager {
        todo!()
    }

    async fn get_file(&self, path: &str) -> anyhow::Result<FileDetail> {
        todo!()
    }

    async fn create_file(&self, path: &str) -> anyhow::Result<FileDetail> {
        todo!()
    }

    async fn is_file_exists(&self, path: &str) -> anyhow::Result<bool> {
        todo!()
    }

    async fn delete_file(&self, path: &str) -> anyhow::Result<()> {
        todo!()
    }
}

pub struct DiskDirectoryManager<P>
    where
        P: PathProvider + Send + Sync
{
    path_provider: P,
}

#[async_trait]
impl<P> DirectoryManager for DiskDirectoryManager<P>
    where
        P: PathProvider + Send + Sync
{
    fn path_provider(&self) -> &dyn PathProvider {
        &self.path_provider
    }

    async fn create_dir(&self, path: &str) -> anyhow::Result<DirectoryDetail> {
        let updated_path = self.path_provider().combine(vec![path])?;

        fs::create_dir(updated_path).await?;

        self.get_dir(path).await
    }

    async fn get_dir(&self, path: &str) -> anyhow::Result<DirectoryDetail> {
        let updated_path = self.path_provider().combine(vec![path])?;


        let metadata = fs::metadata(updated_path.clone()).await?;

        if !metadata.is_dir() {
            bail!("given path is not a dir: {}", path);
        }

        Ok(DirectoryDetail {
            dir_manager: self,
            name: self.path_provider().get_object_name(updated_path.as_str())?,
            path: self.path_provider().get_object_parent_path(updated_path.as_str())?,
        }.clone())
    }

    async fn is_dir_exists(&self, path: &str) -> anyhow::Result<bool> {
        let updated_path = self.path_provider().combine(vec![path])?;

        let metadata = fs::metadata(updated_path).await?;

        Ok(metadata.is_dir())
    }

    async fn delete_dir(&self, path: &str, recursive: bool) -> anyhow::Result<()> {
        let updated_path = self.path_provider().combine(vec![path])?;

        if recursive {
            fs::remove_dir_all(updated_path).await?;
        } else {
            fs::remove_dir(updated_path).await?;
        }

        Ok(())
    }
}

#[derive(Copy, Clone)]
pub struct SystemPathProvider {}

impl SystemPathProvider {
    fn new() -> SystemPathProvider {
        SystemPathProvider {}
    }
}

impl PathProvider for SystemPathProvider {
    fn combine(&self, paths: Vec<&str>) -> anyhow::Result<String> {
        let mut buf = path::PathBuf::new();

        for path in paths {
            buf.push(path)
        }

        match buf.to_str() {
            Some(v) => Ok(normalize_path(v.to_string())),
            None => bail!("empty or invalid paths"),
        }
    }

    fn get_object_name(&self, path: &str) -> anyhow::Result<String> {
        let normalized_path = normalize_path(path.to_string());
        let p = path::Path::new(&normalized_path);

        if let Some(v) = p.file_name() {
            if let Some(v) = v.to_str() {
                return Ok(v.to_string());
            }
        }

        bail!("invalid path: {}", path)
    }

    fn get_object_parent_path(&self, path: &str) -> anyhow::Result<String> {
        let normalized_path = normalize_path(path.to_string());
        let p = path::Path::new(&normalized_path);

        if let Some(v) = p.parent() {
            if let Some(v) = v.to_str() {
                if v == "" {
                    bail!("invalid path: {}", path);
                }
                return Ok(v.to_string());
            }
        }

        bail!("invalid path: {}", path)
    }
}

fn normalize_path(mut path: String) -> String {
    while let Some(pos) = path.find("/..") {
        if pos == 0 || pos == 1 {
            break;
        }

        let tmp_path: String = path.chars().take(pos).collect();
        if let Some(parent_pos) = tmp_path.rfind("/") {
            let a: String = tmp_path.chars().take(parent_pos).collect::<String>();
            let b: String = path.chars().skip(pos + 3).collect::<String>();
            path = a;
            path.push_str(&b);
        }
    }

    while path.contains("/.") {
        path = path.replace("/.", "");
    }

    if path.is_empty() {
        path += "./"
    };

    path
}

#[cfg(test)]
#[path = "./lib_test.rs"]
mod lib_test;