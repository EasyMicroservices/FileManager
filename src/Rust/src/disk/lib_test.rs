use tempfile::{tempdir, TempDir};
use crate::providers::{DirectoryManager, FileManager, PathProvider};
use super::{SystemPathProvider, normalize_path, DiskDirectoryManager, DiskFileManager};

mod test_system_path_provider {
    use super::*;

    #[test]
    fn test_system_path_provider_combine() {
        let provider = SystemPathProvider::new();

        let res = provider.combine(vec!["parent", "dir", "file"]);
        assert!(res.is_ok());
        assert_eq!("parent/dir/file", res.unwrap());

        let res = provider.combine(vec!["parent/dir", "./file"]);
        assert!(res.is_ok());
        assert_eq!("parent/dir/file", res.unwrap());

        let res = provider.combine(vec!["parent", "dir/file"]);
        assert!(res.is_ok());
        assert_eq!("parent/dir/file", res.unwrap());

        let res = provider.combine(vec!["parent/dir/file"]);
        assert!(res.is_ok());
        assert_eq!("parent/dir/file", res.unwrap());

        let res = provider.combine(vec!["/parent", "dir", "file"]);
        assert!(res.is_ok());
        assert_eq!("/parent/dir/file", res.unwrap());

        let res = provider.combine(vec!["/parent/dir/file", "../.."]);
        assert!(res.is_ok());
        assert_eq!("/parent", res.unwrap());

        let res = provider.combine(vec!["parent", "dir", "../file", "../"]);
        assert!(res.is_ok());
        assert_eq!("parent/", res.unwrap());
    }

    #[test]
    fn test_system_path_provider_get_object_name() {
        let provider = SystemPathProvider::new();

        let res = provider.get_object_name("parent/dir/file");
        assert!(res.is_ok());
        assert_eq!("file", res.unwrap());

        let res = provider.get_object_name("parent/dir/");
        assert!(res.is_ok());
        assert_eq!("dir", res.unwrap());

        let res = provider.get_object_name("parent/dir//");
        assert!(res.is_ok());
        assert_eq!("dir", res.unwrap());

        let res = provider.get_object_name("parent/dir/../");
        assert!(res.is_ok());
        assert_eq!("parent", res.unwrap());

        let res = provider.get_object_name("/");
        assert!(res.is_err());
    }

    #[test]
    fn test_system_path_provider_get_object_parent_path() {
        let provider = SystemPathProvider::new();

        let res = provider.get_object_parent_path("parent/dir/file");
        assert!(res.is_ok());
        assert_eq!("parent/dir", res.unwrap());

        let res = provider.get_object_parent_path("parent/dir/");
        assert!(res.is_ok());
        assert_eq!("parent", res.unwrap());

        let res = provider.get_object_parent_path("parent/dir//");
        assert!(res.is_ok());
        assert_eq!("parent", res.unwrap());

        let res = provider.get_object_parent_path("/parent/dir/../");
        assert!(res.is_ok());
        assert_eq!("/", res.unwrap());

        let res = provider.get_object_parent_path("/");
        assert!(res.is_err());

        let res = provider.get_object_parent_path("parent/dir/../");
        assert!(res.is_err());
    }
}

#[test]
fn test_normalize_path() {
    assert_eq!("parent/dir", normalize_path("parent/dir/.".to_string()));

    assert_eq!("parent/dir/.file", normalize_path("parent/dir/.file".to_string()));

    assert_eq!("parent", normalize_path("parent/dir/..".to_string()));

    assert_eq!("parent", normalize_path("parent/dir/file/../..".to_string()));

    assert_eq!("parent/dir/..sub", normalize_path("parent/dir/file/../..sub".to_string()));

    assert_eq!("parent/dir/file", normalize_path("parent/dir/file/./.".to_string()));

    assert_eq!("parent/file", normalize_path("parent/dir/../file".to_string()));

    assert_eq!("parent/dir/file", normalize_path("parent/dir/./file".to_string()));

    assert_eq!("../parent/dir", normalize_path("../parent/dir".to_string()));

    assert_eq!("./parent/dir", normalize_path("./parent/dir".to_string()));

    assert_eq!(".parent/dir", normalize_path(".parent/dir".to_string()));

    assert_eq!("..parent/dir", normalize_path("..parent/dir".to_string()));

    assert_eq!("/parent/dir", normalize_path("/./parent/dir".to_string()));

    assert_eq!("/../parent/dir", normalize_path("/../parent/dir".to_string()));
}

mod test_disk_dir_manager_test {
    use super::*;

    #[test]
    fn test_disk_dir_manager_path_provider() {
        let spp = SystemPathProvider::new();

        let dm = DiskDirectoryManager::new(spp);

        let provider = dm.path_provider().as_any()
            .downcast_ref::<SystemPathProvider>()
            .unwrap();
        assert_eq!(&spp, provider);
    }

    #[tokio::test]
    async fn test_disk_dir_manager_create_dir_async_when_not_exists() {
        let dm = init_disk_dir_manager();

        let (tmp_dir, test_dir, _t) = get_test_dir(dm.path_provider());

        let res = dm.create_dir_async(&test_dir).await;
        assert!(res.is_ok());
        let dd = res.unwrap();
        assert_eq!("test_dir", dd.name);
        assert_eq!(tmp_dir, dd.path);
    }

    #[tokio::test]
    async fn test_disk_dir_manager_create_dir_async_when_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());
        dm.create_dir_async(&test_dir).await.unwrap();

        let res = dm.create_dir_async(&test_dir).await;
        assert!(res.is_err());
    }

    #[tokio::test]
    async fn test_disk_dir_manager_get_dir_async_when_not_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());

        let res = dm.get_dir_async(&test_dir).await;
        assert!(res.is_err());
    }

    #[tokio::test]
    async fn test_disk_dir_manager_get_dir_async_when_exists() {
        let dm = init_disk_dir_manager();

        let (tmp_dir, test_dir, _t) = get_test_dir(dm.path_provider());
        dm.create_dir_async(&test_dir).await.unwrap();

        let res = dm.get_dir_async(&test_dir).await;
        assert!(res.is_ok());
        let dd = res.unwrap();
        assert_eq!("test_dir", dd.name);
        assert_eq!(tmp_dir, dd.path);
    }

    #[tokio::test]
    async fn test_disk_dir_manager_is_dir_exists_async_when_not_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());

        let res = dm.is_dir_exists_async(&test_dir).await;
        assert!(res.is_ok());
        assert!(!res.unwrap());
    }

    #[tokio::test]
    async fn test_disk_dir_manager_is_dir_exists_async_when_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());
        dm.create_dir_async(&test_dir).await.unwrap();

        let res = dm.is_dir_exists_async(&test_dir).await;
        assert!(res.is_ok());
        assert!(res.unwrap());
    }

    #[tokio::test]
    async fn test_disk_dir_manager_delete_dir_async_when_not_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());

        let res = dm.delete_dir_async(&test_dir, true).await;
        assert!(res.is_err());
    }

    #[tokio::test]
    async fn test_disk_dir_manager_delete_dir_async_when_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());
        dm.create_dir_async(&test_dir).await.unwrap();

        let res = dm.delete_dir_async(&test_dir, true).await;
        assert!(res.is_ok());

        let (_, test_dir, _t1) = get_test_dir(dm.path_provider());
        dm.create_dir_async(&test_dir).await.unwrap();

        let res = dm.delete_dir_async(&test_dir, false).await;
        assert!(res.is_ok());
    }

    #[test]
    fn test_disk_dir_manager_create_dir_when_not_exists() {
        let dm = init_disk_dir_manager();

        let (tmp_dir, test_dir, _t) = get_test_dir(dm.path_provider());

        let res = dm.create_dir(&test_dir);
        assert!(res.is_ok());
        let dd = res.unwrap();
        assert_eq!("test_dir", dd.name);
        assert_eq!(tmp_dir, dd.path);
    }

    #[test]
    fn test_disk_dir_manager_create_dir_when_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());
        dm.create_dir(&test_dir).unwrap();

        let res = dm.create_dir(&test_dir);
        assert!(res.is_err());
    }

    #[test]
    fn test_disk_dir_manager_get_dir_when_not_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());

        let res = dm.get_dir(&test_dir);
        assert!(res.is_err());
    }

    #[test]
    fn test_disk_dir_manager_get_dir_when_exists() {
        let dm = init_disk_dir_manager();

        let (tmp_dir, test_dir, _t) = get_test_dir(dm.path_provider());
        dm.create_dir(&test_dir).unwrap();

        let res = dm.get_dir(&test_dir);
        assert!(res.is_ok());
        let dd = res.unwrap();
        assert_eq!("test_dir", dd.name);
        assert_eq!(tmp_dir, dd.path);
    }

    #[test]
    fn test_disk_dir_manager_is_dir_exists_when_not_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());

        let res = dm.is_dir_exists(&test_dir);
        assert!(res.is_ok());
        assert!(!res.unwrap());
    }

    #[test]
    fn test_disk_dir_manager_is_dir_exists_when_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());
        dm.create_dir(&test_dir).unwrap();

        let res = dm.is_dir_exists(&test_dir);
        assert!(res.is_ok());
        assert!(res.unwrap());
    }

    #[test]
    fn test_disk_dir_manager_delete_dir_when_not_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());

        let res = dm.delete_dir(&test_dir, true);
        assert!(res.is_err());
    }

    #[test]
    fn test_disk_dir_manager_delete_dir_when_exists() {
        let dm = init_disk_dir_manager();

        let (_, test_dir, _t) = get_test_dir(dm.path_provider());
        dm.create_dir(&test_dir).unwrap();

        let res = dm.delete_dir(&test_dir, true);
        assert!(res.is_ok());

        let (_, test_dir, _t1) = get_test_dir(dm.path_provider());
        dm.create_dir(&test_dir).unwrap();

        let res = dm.delete_dir(&test_dir, false);
        assert!(res.is_ok());
    }
}

mod test_disk_file_manager_test {
    use super::*;

    #[test]
    fn test_disk_file_manager_path_provider() {
        let spp = SystemPathProvider::new();
        let dm = DiskDirectoryManager::new(spp);

        let fm = DiskFileManager::new(dm);

        let provider = fm.path_provider().as_any()
            .downcast_ref::<SystemPathProvider>()
            .unwrap();

        assert_eq!(&spp, provider);
    }

    #[test]
    fn test_disk_file_manager_dir_manager() {
        let dm = init_disk_dir_manager();
        let fm = DiskFileManager::new(dm);

        let dir_manager = fm.dir_manager().as_any()
            .downcast_ref::<DiskDirectoryManager<SystemPathProvider>>()
            .unwrap();

        assert_eq!(&dm, dir_manager);
    }

    #[tokio::test]
    async fn test_disk_file_manager_create_file_async_when_not_exists() {
        let fm = init_disk_file_manager();

        let (tmp_dir, test_file, _t) = get_test_file(fm.path_provider());

        let res = fm.create_file_async(&test_file).await;
        assert!(res.is_ok());
        let fd = res.unwrap();
        assert_eq!("test_file", fd.name);
        assert_eq!(tmp_dir, fd.path);
        assert_eq!(0, fd.len);
    }

    #[tokio::test]
    async fn test_disk_file_manager_create_file_async_when_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());
        fm.create_file_async(&test_file).await.unwrap();

        let res = fm.create_file_async(&test_file).await;
        assert!(res.is_err());
    }

    #[tokio::test]
    async fn test_disk_file_manager_get_file_async_when_not_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());

        let res = fm.get_file_async(&test_file).await;
        assert!(res.is_err());
    }

    #[tokio::test]
    async fn test_disk_file_manager_get_file_async_when_exists() {
        let fm = init_disk_file_manager();

        let (tmp_dir, test_file, _t) = get_test_file(fm.path_provider());
        fm.create_file_async(&test_file).await.unwrap();

        let res = fm.get_file_async(&test_file).await;
        assert!(res.is_ok());
        let fd = res.unwrap();
        assert_eq!("test_file", fd.name);
        assert_eq!(tmp_dir, fd.path);
        assert_eq!(0, fd.len);
    }

    #[tokio::test]
    async fn test_disk_file_manager_is_file_exists_async_when_not_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());

        let res = fm.is_file_exists_async(&test_file).await;
        assert!(res.is_ok());
        assert!(!res.unwrap());
    }

    #[tokio::test]
    async fn test_disk_file_manager_is_file_exists_async_when_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());
        fm.create_file_async(&test_file).await.unwrap();

        let res = fm.is_file_exists_async(&test_file).await;
        assert!(res.is_ok());
        assert!(res.unwrap());
    }

    #[tokio::test]
    async fn test_disk_file_manager_delete_file_async_when_not_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());

        let res = fm.delete_file_async(&test_file).await;
        assert!(res.is_err());
    }

    #[tokio::test]
    async fn test_disk_file_manager_delete_file_async_when_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());
        fm.create_file_async(&test_file).await.unwrap();

        let res = fm.delete_file_async(&test_file).await;
        assert!(res.is_ok());
    }
    
    #[test]
    fn test_disk_file_manager_create_file_when_not_exists() {
        let fm = init_disk_file_manager();

        let (tmp_dir, test_file, _t) = get_test_file(fm.path_provider());

        let res = fm.create_file(&test_file);
        assert!(res.is_ok());
        let fd = res.unwrap();
        assert_eq!("test_file", fd.name);
        assert_eq!(tmp_dir, fd.path);
        assert_eq!(0, fd.len);
    }

    #[test]
    fn test_disk_file_manager_create_file_when_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());
        fm.create_file(&test_file).unwrap();

        let res = fm.create_file(&test_file);
        assert!(res.is_err());
    }

    #[test]
    fn test_disk_file_manager_get_file_when_not_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());

        let res = fm.get_file(&test_file);
        assert!(res.is_err());
    }

    #[test]
    fn test_disk_file_manager_get_file_when_exists() {
        let fm = init_disk_file_manager();

        let (tmp_dir, test_file, _t) = get_test_file(fm.path_provider());
        fm.create_file(&test_file).unwrap();

        let res = fm.get_file(&test_file);
        assert!(res.is_ok());
        let fd = res.unwrap();
        assert_eq!("test_file", fd.name);
        assert_eq!(tmp_dir, fd.path);
        assert_eq!(0, fd.len);
    }

    #[test]
    fn test_disk_file_manager_is_file_exists_when_not_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());

        let res = fm.is_file_exists(&test_file);
        assert!(res.is_ok());
        assert!(!res.unwrap());
    }

    #[test]
    fn test_disk_file_manager_is_file_exists_when_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());
        fm.create_file(&test_file).unwrap();

        let res = fm.is_file_exists(&test_file);
        assert!(res.is_ok());
        assert!(res.unwrap());
    }

    #[test]
    fn test_disk_file_manager_delete_file_when_not_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());

        let res = fm.delete_file(&test_file);
        assert!(res.is_err());
    }

    #[test]
    fn test_disk_file_manager_delete_file_when_exists() {
        let fm = init_disk_file_manager();

        let (_, test_file, _t) = get_test_file(fm.path_provider());
        fm.create_file(&test_file).unwrap();

        let res = fm.delete_file(&test_file);
        assert!(res.is_ok());
    }
}

fn get_test_dir(provider: &dyn PathProvider) -> (String, String, TempDir) {
    let t = tempdir().unwrap();
    let tmp_dir = t.path().to_str().unwrap();
    let test_dir = provider.combine(vec![tmp_dir, "test_dir"]).unwrap();

    (tmp_dir.to_string(), test_dir, t)
}

fn get_test_file(provider: &dyn PathProvider) -> (String, String, TempDir) {
    let t = tempdir().unwrap();
    let tmp_dir = t.path().to_str().unwrap();
    let test_file = provider.combine(vec![tmp_dir, "test_file"]).unwrap();

    (tmp_dir.to_string(), test_file, t)
}

fn init_disk_dir_manager() -> DiskDirectoryManager<SystemPathProvider> {
    DiskDirectoryManager::new(SystemPathProvider::new())
}

fn init_disk_file_manager() -> DiskFileManager<DiskDirectoryManager<SystemPathProvider>> {
    DiskFileManager::new(init_disk_dir_manager())
}