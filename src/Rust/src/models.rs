use crate::{DirectoryManager, FileManager};
use anyhow::Result;

pub struct FileDetail<'a> {
    file_manager: &'a dyn FileManager,
    name: &'a str,
    path: &'a str,
    len: i64,
}

impl FileDetail<'_> {
    pub fn new(file_manager: &dyn FileManager) -> FileDetail {
        FileDetail {
            file_manager,
            name: "",
            path: "",
            len: 0,
        }
    }

    pub fn full_path(&self) -> Result<String> {
        self.file_manager.path_provider().combine(
            vec![self.name, self.path]
        )
    }
}

pub struct DirectoryDetail<'a> {
    dir_manager: &'a dyn DirectoryManager,
    name: &'a str,
    path: &'a str,
}

impl DirectoryDetail<'_> {
    pub fn new(dir_manager: &dyn DirectoryManager) -> DirectoryDetail {
        DirectoryDetail {
            dir_manager,
            name: "",
            path: "",
        }
    }

    pub fn full_path(&self) -> Result<String> {
        self.dir_manager.path_provider().combine(
            vec![self.name, self.path]
        )
    }
}
