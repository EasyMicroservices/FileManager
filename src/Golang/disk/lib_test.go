package disk

import (
	"github.com/stretchr/testify/assert"
	"os"
	"testing"
)

func initDiskFileManager() *DiskFileManager {
	provider := SystemPathProvider{}
	dirManager := InitDiskDirectoryManager(&provider)

	fileManager := InitDiskFileManager(&provider, &dirManager)

	return &fileManager
}

func initDiskDirManager() *DiskDirectoryManager {
	provider := SystemPathProvider{}
	dirManager := InitDiskDirectoryManager(&provider)

	return &dirManager
}

func TestSystemPathProvider_CombineCombine(t *testing.T) {
	var res string
	var err error
	provider := new(SystemPathProvider)
	res, err = provider.Combine("parent", "dir", "file")
	assert.Nil(t, err)
	assert.Equal(t, "parent/dir/file", res)

	res, err = provider.Combine("parent/dir", "file")
	assert.Nil(t, err)
	assert.Equal(t, "parent/dir/file", res)

	res, err = provider.Combine("parent", "dir/file")
	assert.Nil(t, err)
	assert.Equal(t, "parent/dir/file", res)

	res, err = provider.Combine("parent/dir/file")
	assert.Nil(t, err)
	assert.Equal(t, "parent/dir/file", res)

	res, err = provider.Combine("/parent", "dir", "file")
	assert.Nil(t, err)
	assert.Equal(t, "/parent/dir/file", res)

	res, err = provider.Combine("/parent", "/dir", "file")
	assert.Nil(t, err)
	assert.Equal(t, "/parent/dir/file", res)

	res, err = provider.Combine("/parent/dir/file", "../..")
	assert.Nil(t, err)
	assert.Equal(t, "/parent", res)
}

func TestNormalizePath(t *testing.T) {
	var res string

	res = normalizePath("dir/file")
	assert.Equal(t, "dir/file", res)

	res = normalizePath("dir/file/")
	assert.Equal(t, "dir/file", res)

	res = normalizePath("dir/file//")
	assert.Equal(t, "dir/file", res)
}

func TestSystemPathProvider_GetObjectName(t *testing.T) {
	var res string
	var err error

	provider := new(SystemPathProvider)
	res, err = provider.GetObjectName("parent/dir/file")
	assert.Nil(t, err)
	assert.Equal(t, "file", res)

	res, err = provider.GetObjectName("parent/dir/")
	assert.Nil(t, err)
	assert.Equal(t, "dir", res)

	res, err = provider.GetObjectName("parent/dir//")
	assert.Nil(t, err)
	assert.Equal(t, "dir", res)

	res, err = provider.GetObjectName("parent/dir/../")
	assert.Nil(t, err)
	assert.Equal(t, "parent", res)

	res, err = provider.GetObjectName("/")
	assert.Equal(t, "", res)
	assert.NotNil(t, res)
	assert.Contains(t, err.Error(), "invalid path:")
}

func TestSystemPathProvider_GetObjectParentPath(t *testing.T) {
	var res string
	var err error

	provider := new(SystemPathProvider)
	res, err = provider.GetObjectParentPath("parent/dir/file")
	assert.Nil(t, err)
	assert.Equal(t, "parent/dir/", res)

	res, err = provider.GetObjectParentPath("parent/dir/")
	assert.Nil(t, err)
	assert.Equal(t, "parent/", res)

	res, err = provider.GetObjectParentPath("parent/dir//")
	assert.Nil(t, err)
	assert.Equal(t, "parent/", res)

	res, err = provider.GetObjectParentPath("/parent/dir/../")
	assert.Nil(t, err)
	assert.Equal(t, "/", res)

	res, err = provider.GetObjectParentPath("/")
	assert.Equal(t, "", res)
	assert.NotNil(t, res)
	assert.Contains(t, err.Error(), "invalid path:")

	res, err = provider.GetObjectParentPath("parent/dir/../")
	assert.Equal(t, "", res)
	assert.NotNil(t, res)
	assert.Contains(t, err.Error(), "invalid path:")
}

func TestDiskFileManager_GetPathProvider(t *testing.T) {
	provider := SystemPathProvider{}
	dirManager := InitDiskDirectoryManager(&provider)

	fileManager := InitDiskFileManager(&provider, &dirManager)

	assert.Equal(t, &provider, fileManager.GetPathProvider())
}

func TestDiskFileManager_GetDirectoryManager(t *testing.T) {
	provider := SystemPathProvider{}
	dirManager := InitDiskDirectoryManager(&provider)

	fileManager := InitDiskFileManager(&provider, &dirManager)

	assert.Equal(t, &dirManager, fileManager.GetDirectoryManager())
}

func TestDiskFileManager_CreateFile_WhenNotExists(t *testing.T) {
	fm := initDiskFileManager()

	// Create file...
	fd, err := fm.CreateFile("test_file")
	assert.Nil(t, err)
	assert.Equal(t, "test_file", fd.Name)
	assert.Equal(t, "", fd.Path)
	assert.Equal(t, int64(0), fd.Length)

	fullPath, err := fd.FullPath()
	assert.Nil(t, err)
	assert.Equal(t, "test_file", fullPath)

	// Cleanup
	_ = fm.DeleteFile("./test_file")
}

func TestDiskFileManager_CreateFile_WhenExists(t *testing.T) {
	fm := initDiskFileManager()

	// Preparing file...
	_, _ = fm.CreateFile("./test_file")

	// Recreate same file
	fd, err := fm.CreateFile("test_file")
	assert.Nil(t, fd)
	assert.NotNil(t, err)
	assert.True(t, os.IsExist(err))

	// Cleanup
	_ = fm.DeleteFile("./test_file")
}

func TestDiskFileManager_GetFile_WhenNotExists(t *testing.T) {
	fm := initDiskFileManager()

	fd, err := fm.GetFile("./test_file")
	assert.Nil(t, fd)
	assert.NotNil(t, err)
	assert.True(t, os.IsNotExist(err))
}

func TestDiskFileManager_GetFile_WhenExists(t *testing.T) {
	fm := initDiskFileManager()

	// Prepare file...
	_, _ = fm.CreateFile("./test_file")

	fd, err := fm.GetFile("test_file")
	assert.Nil(t, err)
	assert.Equal(t, "test_file", fd.Name)
	assert.Equal(t, "", fd.Path)
	assert.Equal(t, int64(0), fd.Length)

	fullPath, err := fd.FullPath()
	assert.Nil(t, err)
	assert.Equal(t, "test_file", fullPath)

	// Cleanup...
	_ = fm.DeleteFile("test_file")
}

func TestDiskFileManager_FileExists_WhenNotExists(t *testing.T) {
	fm := initDiskFileManager()

	exists, err := fm.FileExists("./test_file")
	assert.Nil(t, err)
	assert.False(t, exists)
}

func TestDiskFileManager_FileExists_WhenExists(t *testing.T) {
	fm := initDiskFileManager()

	// Prepare file...
	_, _ = fm.CreateFile("./test_file")

	exists, err := fm.FileExists("test_file")
	assert.Nil(t, err)
	assert.True(t, exists)

	// Cleanup...
	_ = fm.DeleteFile("./test_file")
}

func TestDiskFileManager_DeleteFile_WhenNotExists(t *testing.T) {
	fm := initDiskFileManager()

	err := fm.DeleteFile("./test_file")
	assert.NotNil(t, err)
	assert.True(t, os.IsNotExist(err))
}

func TestDiskFileManager_DeleteFile_WhenExists(t *testing.T) {
	fm := initDiskFileManager()

	// Prepare file...
	_, _ = fm.CreateFile("./test_file")

	err := fm.DeleteFile("test_file")
	assert.Nil(t, err)
}

func TestDiskDirectoryManager_GetPathProvider(t *testing.T) {
	provider := SystemPathProvider{}

	dm := InitDiskDirectoryManager(&provider)

	assert.Equal(t, &provider, dm.GetPathProvider())
}

func TestDiskDirectoryManager_CreateDir_WhenNotExists(t *testing.T) {
	dm := initDiskDirManager()

	dd, err := dm.CreateDir("test_dir")
	assert.Nil(t, err)
	assert.Equal(t, "test_dir", dd.Name)
	assert.Equal(t, "", dd.Path)

	fullPath, err := dd.FullPath()
	assert.Nil(t, err)
	assert.Equal(t, "test_dir", fullPath)

	// Cleanup...
	_ = dm.DeleteDir("./test_dir")
}

func TestDiskDirectoryManager_CreateDir_WhenExists(t *testing.T) {
	dm := initDiskDirManager()

	// Prepare dir...
	_, _ = dm.CreateDir("./test_dir")

	dd, err := dm.CreateDir("test_dir")
	assert.Nil(t, dd)
	assert.NotNil(t, err)
	assert.True(t, os.IsExist(err))

	// Cleanup...
	_ = dm.DeleteDir("./test_dir")
}

func TestDiskDirectoryManager_GetDir_WhenNotExists(t *testing.T) {
	dm := initDiskDirManager()

	dd, err := dm.GetDir("test_dir")
	assert.Nil(t, dd)
	assert.NotNil(t, err)
	assert.True(t, os.IsNotExist(err))
}

func TestDiskDirectoryManager_GetDir_WhenExists(t *testing.T) {
	dm := initDiskDirManager()

	// Prepare dir...
	_, _ = dm.CreateDir("./test_dir")

	dd, err := dm.GetDir("test_dir")
	assert.Nil(t, err)
	assert.Equal(t, "test_dir", dd.Name)
	assert.Equal(t, "", dd.Path)

	fullPath, err := dd.FullPath()
	assert.Nil(t, err)
	assert.Equal(t, "test_dir", fullPath)

	// Cleanup...
	_ = dm.DeleteDir("test_dir")
}

func TestDiskDirectoryManager_DirExists_WhenNonExists(t *testing.T) {
	dm := initDiskDirManager()

	exists, err := dm.DirExists("test_dir")
	assert.Nil(t, err)
	assert.False(t, exists)
}

func TestDiskDirectoryManager_DirExists_WhenExists(t *testing.T) {
	dm := initDiskDirManager()

	// Prepare dir...
	_, _ = dm.CreateDir("test_dir")

	exists, err := dm.DirExists("test_dir")
	assert.Nil(t, err)
	assert.True(t, exists)

	// Cleanup...
	_ = dm.DeleteDir("test_dir")
}

func TestDiskDirectoryManager_DeleteDir_WhenNotExists(t *testing.T) {
	dm := initDiskDirManager()

	err := dm.DeleteDir("test_dir")
	assert.NotNil(t, err)
	assert.True(t, os.IsNotExist(err))
}

func TestDiskDirectoryManager_DeleteDir_WhenExists(t *testing.T) {
	dm := initDiskDirManager()

	// Prepare dir...
	_, _ = dm.CreateDir("test_dir")

	err := dm.DeleteDir("test_dir")
	assert.Nil(t, err)
}
