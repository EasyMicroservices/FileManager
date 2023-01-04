use tokio::test;
use super::{SystemPathProvider, PathProvider, normalize_path};

#[test]
async fn test_system_path_provider_combine() {
    let provider = SystemPathProvider::new();

    let res = provider.combine(vec!["parent", "dir", "file"]);
    println!("{:?}", res);
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
async fn test_system_path_provider_get_object_name() {
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
async fn test_system_path_provider_get_object_parent_path() {
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

#[test]
async fn test_normalize_path() {
    assert_eq!("parent/dir", normalize_path("parent/dir/.".to_string()));

    assert_eq!("parent", normalize_path("parent/dir/..".to_string()));

    assert_eq!("parent/file", normalize_path("parent/dir/../file".to_string()));

    assert_eq!("parent/dir/file", normalize_path("parent/dir/./file".to_string()));
    
    assert_eq!("../parent/dir", normalize_path("../parent/dir".to_string()));

    assert_eq!("./parent/dir", normalize_path("./parent/dir".to_string()));
}
