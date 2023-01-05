use anyhow::Result;

use crate::{DirectoryManager, FileManager};

pub struct FileDetail<'a> {
    pub file_manager: &'a dyn FileManager,
    pub name: String,
    pub path: String,
    pub len: u64,
}

impl FileDetail<'_> {
    pub fn new(file_manager: &dyn FileManager) -> FileDetail {
        FileDetail {
            file_manager,
            name: String::new(),
            path: String::new(),
            len: 0,
        }
    }

    pub fn full_path(&self) -> Result<String> {
        self.file_manager.path_provider().combine(
            vec![&self.name, &self.path]
        )
    }
}

#[derive(Clone)]
pub struct DirectoryDetail<'a> {
    pub dir_manager: &'a dyn DirectoryManager,
    pub name: String,
    pub path: String,
}

impl DirectoryDetail<'_> {
    pub fn new(dir_manager: &dyn DirectoryManager) -> DirectoryDetail {
        DirectoryDetail {
            dir_manager,
            name: String::new(),
            path: String::new(),
        }
    }

    pub fn full_path(&self) -> Result<String> {
        self.dir_manager.path_provider().combine(
            vec![&self.name, &self.path]
        )
    }
}
