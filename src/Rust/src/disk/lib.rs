use std::path;
use anyhow::bail;
use async_trait::async_trait;
use tokio::fs;
use std::fs as stdfs;
use std::io as stdio;

use crate::models::{DirectoryDetail, FileDetail};
use crate::providers::{PathProvider, DirectoryManager, FileManager};

#[derive(Copy, Clone, Debug, Eq, PartialEq)]
pub struct DiskFileManager<D>
    where
        D: DirectoryManager + Send + Sync
{
    dir_manager: D,
}

impl<D> DiskFileManager<D>
    where
        D: DirectoryManager + Send + Sync
{
    pub fn new(dir_manager: D) -> DiskFileManager<D> {
        DiskFileManager {
            dir_manager
        }
    }
}

#[async_trait]
impl<D> FileManager for DiskFileManager<D>
    where
        D: DirectoryManager + Send + Sync
{
    fn path_provider(&self) -> &dyn PathProvider {
        self.dir_manager.path_provider()
    }

    fn dir_manager(&self) -> &dyn DirectoryManager {
        &self.dir_manager
    }

    async fn get_file_async(&self, path: &str) -> anyhow::Result<FileDetail> {
        let updated_path = self.path_provider().combine(vec![path])?;

        let metadata = fs::metadata(updated_path.clone()).await?;

        if !metadata.is_file() {
            return Err(std::io::Error::from(std::io::ErrorKind::NotFound).into());
        }

        return Ok(FileDetail {
            file_manager: self,
            name: self.path_provider().get_object_name(&updated_path)?,
            path: self.path_provider().get_object_parent_path(&updated_path)?,
            len: metadata.len(),
        });
    }

    async fn create_file_async(&self, path: &str) -> anyhow::Result<FileDetail> {
        let exists = self.is_file_exists_async(path).await?;
        if exists {
            return Err(std::io::Error::from(std::io::ErrorKind::AlreadyExists).into());
        }

        let updated_path = self.path_provider().combine(vec![path])?;

        fs::File::create(updated_path).await?;

        return self.get_file_async(path).await;
    }

    async fn is_file_exists_async(&self, path: &str) -> anyhow::Result<bool> {
        let updated_path = self.path_provider().combine(vec![path])?;

        let metadata = match fs::metadata(updated_path).await {
            Ok(v) => v,
            Err(e) => {
                if e.kind() == std::io::ErrorKind::NotFound {
                    return Ok(false);
                }
                return Err(e.into());
            }
        };

        Ok(metadata.is_file())
    }

    async fn delete_file_async(&self, path: &str) -> anyhow::Result<()> {
        let updated_path = self.path_provider().combine(vec![path])?;

        fs::remove_file(updated_path).await?;

        Ok(())
    }

    fn get_file(&self, path: &str) -> anyhow::Result<FileDetail> {
        let updated_path = self.path_provider().combine(vec![path])?;

        let metadata = stdfs::metadata(updated_path.clone())?;

        if !metadata.is_file() {
            return Err(stdio::Error::from(stdio::ErrorKind::NotFound).into());
        }

        Ok(FileDetail {
            file_manager: self,
            name: self.path_provider().get_object_name(&updated_path)?,
            path: self.path_provider().get_object_parent_path(&updated_path)?,
            len: metadata.len(),
        })
    }

    fn create_file(&self, path: &str) -> anyhow::Result<FileDetail> {
        if self.is_file_exists(path)? {
            return Err(stdio::Error::from(stdio::ErrorKind::AlreadyExists).into());
        }

        let updated_path = self.path_provider().combine(vec![path])?;

        stdfs::File::create(updated_path)?;

        self.get_file(path)
    }

    fn is_file_exists(&self, path: &str) -> anyhow::Result<bool> {
        let updated_path = self.path_provider().combine(vec![path])?;

        let metadata = match stdfs::metadata(updated_path) {
            Ok(v) => v,
            Err(e) => {
                if e.kind() == stdio::ErrorKind::NotFound {
                    return Ok(false);
                }
                return Err(e.into());
            }
        };

        Ok(metadata.is_file())
    }

    fn delete_file(&self, path: &str) -> anyhow::Result<()> {
        let updated_path = self.path_provider().combine(vec![path])?;

        stdfs::remove_file(updated_path)?;

        Ok(())
    }
}

#[derive(Copy, Clone, Debug, Eq, PartialEq)]
pub struct DiskDirectoryManager<P>
    where
        P: PathProvider + Send + Sync
{
    path_provider: P,
}

impl<P> DiskDirectoryManager<P>
    where
        P: PathProvider + Send + Sync
{
    pub fn new(path_provider: P) -> DiskDirectoryManager<P> {
        DiskDirectoryManager {
            path_provider
        }
    }
}

#[async_trait]
impl<P> DirectoryManager for DiskDirectoryManager<P>
    where
        P: PathProvider + Send + Sync
{
    fn path_provider(&self) -> &dyn PathProvider {
        &self.path_provider
    }

    async fn create_dir_async(&self, path: &str) -> anyhow::Result<DirectoryDetail> {
        let updated_path = self.path_provider().combine(vec![path])?;

        fs::create_dir(updated_path).await?;

        self.get_dir_async(path).await
    }

    async fn get_dir_async(&self, path: &str) -> anyhow::Result<DirectoryDetail> {
        let updated_path = self.path_provider().combine(vec![path])?;

        let metadata = fs::metadata(updated_path.clone()).await?;

        if !metadata.is_dir() {
            return Err(std::io::Error::from(std::io::ErrorKind::AlreadyExists).into());
        }

        Ok(DirectoryDetail {
            dir_manager: self,
            name: self.path_provider().get_object_name(&updated_path)?,
            path: self.path_provider().get_object_parent_path(&updated_path)?,
        }.clone())
    }

    async fn is_dir_exists_async(&self, path: &str) -> anyhow::Result<bool> {
        let updated_path = self.path_provider().combine(vec![path])?;

        let metadata = fs::metadata(updated_path).await;

        let metadata = match metadata {
            Ok(v) => v,
            Err(e) => {
                if e.kind() == std::io::ErrorKind::NotFound {
                    return Ok(false);
                }
                return Err(e.into());
            }
        };

        Ok(metadata.is_dir())
    }

    async fn delete_dir_async(&self, path: &str, recursive: bool) -> anyhow::Result<()> {
        let updated_path = self.path_provider().combine(vec![path])?;

        if recursive {
            fs::remove_dir_all(updated_path).await?;
        } else {
            fs::remove_dir(updated_path).await?;
        }

        Ok(())
    }

    fn create_dir(&self, path: &str) -> anyhow::Result<DirectoryDetail> {
        let updated_path = self.path_provider().combine(vec![path])?;

        stdfs::create_dir(updated_path)?;

        self.get_dir(path)
    }

    fn get_dir(&self, path: &str) -> anyhow::Result<DirectoryDetail> {
        let updated_path = self.path_provider().combine(vec![path])?;

        let metadata = stdfs::metadata(updated_path.clone())?;

        if !metadata.is_dir() {
            return Err(stdio::Error::from(stdio::ErrorKind::AlreadyExists).into());
        }

        Ok(DirectoryDetail {
            dir_manager: self,
            name: self.path_provider().get_object_name(&updated_path)?,
            path: self.path_provider().get_object_parent_path(&updated_path)?,
        })
    }

    fn is_dir_exists(&self, path: &str) -> anyhow::Result<bool> {
        let updated_path = self.path_provider().combine(vec![path])?;

        let metadata = match stdfs::metadata(updated_path) {
            Ok(v) => v,
            Err(e) => {
                if e.kind() == stdio::ErrorKind::NotFound {
                    return Ok(false);
                }
                return Err(e.into());
            }
        };

        Ok(metadata.is_dir())
    }

    fn delete_dir(&self, path: &str, recursive: bool) -> anyhow::Result<()> {
        let updated_path = self.path_provider().combine(vec![path])?;

        if recursive {
            stdfs::remove_dir_all(updated_path)?;
        } else {
            stdfs::remove_dir(updated_path)?;
        }

        Ok(())
    }
}

#[derive(Copy, Clone, Debug, Eq, PartialEq)]
pub struct SystemPathProvider {}

impl SystemPathProvider {
    pub fn new() -> SystemPathProvider {
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
    let mut i: usize = 0;
    while let Some(pos) = path.chars().skip(i).collect::<String>().find("/..") {
        if let Some(v) = path.chars().nth(pos + 3) {
            if v != '/' {
                i = pos + 1;
                continue;
            }
        }

        let tmp_path: String = path.chars().take(pos).collect();
        if let Some(parent_pos) = tmp_path.rfind("/") {
            let a: String = tmp_path.chars().take(parent_pos).collect::<String>();
            let b: String = path.chars().skip(pos + 3).collect::<String>();
            path = a;
            path.push_str(&b);
        } else {
            i = pos + 1;
        }
    }

    i = 0;
    while let Some(pos) = path.chars().skip(i).collect::<String>().find("/.") {
        if let Some(v) = path.chars().nth(pos + 2) {
            if v == '/' {
                path = path.replace("/.", "");
            }
        } else {
            path = path.replace("/.", "");
        }
        i = pos + 1;
    }

    if path.is_empty() {
        path += "./"
    };

    path
}

#[cfg(test)]
#[path = "./lib_test.rs"]
mod lib_test;