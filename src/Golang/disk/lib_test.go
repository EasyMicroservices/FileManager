package disk

import (
	"github.com/stretchr/testify/assert"
	"os"
	"testing"
)

func initDiskFileManager() *DiskFileManager {
	fileManager := InitDiskFileManager(
		initDiskDirManager(),
	)

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

	fileManager := InitDiskFileManager(&dirManager)

	assert.Equal(t, &provider, fileManager.GetPathProvider())
}

func TestDiskFileManager_GetDirectoryManager(t *testing.T) {
	provider := SystemPathProvider{}
	dirManager := InitDiskDirectoryManager(&provider)

	fileManager := InitDiskFileManager(&dirManager)

	assert.Equal(t, &dirManager, fileManager.GetDirectoryManager())
}

func TestDiskFileManager_CreateFile_WhenNotExists(t *testing.T) {
	fm := initDiskFileManager()

	// Create file...
	tmpDir := t.TempDir()
	testFile, _ := fm.GetPathProvider().Combine(tmpDir, "test_file")
	fd, err := fm.CreateFile(testFile)
	assert.Nil(t, err)
	assert.Equal(t, "test_file", fd.Name)
	assert.Equal(t, tmpDir+"/", fd.Path)
	assert.Equal(t, int64(0), fd.Length)

	fullPath, err := fd.FullPath()
	assert.Nil(t, err)
	assert.Equal(t, testFile, fullPath)
}

func TestDiskFileManager_CreateFile_WhenExists(t *testing.T) {
	fm := initDiskFileManager()

	// Preparing file...
	testFile, _ := fm.GetPathProvider().Combine(t.TempDir(), "test_file")
	_, _ = fm.CreateFile(testFile)

	// Recreate same file
	fd, err := fm.CreateFile(testFile)
	assert.Nil(t, fd)
	assert.NotNil(t, err)
	assert.True(t, os.IsExist(err))
}

func TestDiskFileManager_GetFile_WhenNotExists(t *testing.T) {
	fm := initDiskFileManager()

	testFile, _ := fm.GetPathProvider().Combine(t.TempDir(), "test_file")
	fd, err := fm.GetFile(testFile)
	assert.Nil(t, fd)
	assert.NotNil(t, err)
	assert.True(t, os.IsNotExist(err))
}

func TestDiskFileManager_GetFile_WhenExists(t *testing.T) {
	fm := initDiskFileManager()

	// Prepare file...
	tmpDir := t.TempDir()
	testFile, _ := fm.GetPathProvider().Combine(tmpDir, "test_file")
	_, _ = fm.CreateFile(testFile)

	fd, err := fm.GetFile(testFile)
	assert.Nil(t, err)
	assert.Equal(t, "test_file", fd.Name)
	assert.Equal(t, tmpDir+"/", fd.Path)
	assert.Equal(t, int64(0), fd.Length)

	fullPath, err := fd.FullPath()
	assert.Nil(t, err)
	assert.Equal(t, testFile, fullPath)
}

func TestDiskFileManager_FileExists_WhenNotExists(t *testing.T) {
	fm := initDiskFileManager()

	testFile, _ := fm.GetPathProvider().Combine(t.TempDir(), "test_file")
	exists, err := fm.FileExists(testFile)
	assert.Nil(t, err)
	assert.False(t, exists)
}

func TestDiskFileManager_FileExists_WhenExists(t *testing.T) {
	fm := initDiskFileManager()

	// Prepare file...
	testFile, _ := fm.GetPathProvider().Combine(t.TempDir(), "test_file")
	_, _ = fm.CreateFile(testFile)

	exists, err := fm.FileExists(testFile)
	assert.Nil(t, err)
	assert.True(t, exists)
}

func TestDiskFileManager_DeleteFile_WhenNotExists(t *testing.T) {
	fm := initDiskFileManager()

	testFile, _ := fm.GetPathProvider().Combine(t.TempDir(), "test_file")
	err := fm.DeleteFile(testFile)
	assert.NotNil(t, err)
	assert.True(t, os.IsNotExist(err))
}

func TestDiskFileManager_DeleteFile_WhenExists(t *testing.T) {
	fm := initDiskFileManager()

	// Prepare file...
	testFile, _ := fm.GetPathProvider().Combine(t.TempDir(), "test_file")
	_, _ = fm.CreateFile(testFile)

	err := fm.DeleteFile(testFile)
	assert.Nil(t, err)
}

func TestDiskDirectoryManager_GetPathProvider(t *testing.T) {
	provider := SystemPathProvider{}

	dm := InitDiskDirectoryManager(&provider)

	assert.Equal(t, &provider, dm.GetPathProvider())
}

func TestDiskDirectoryManager_CreateDir_WhenNotExists(t *testing.T) {
	dm := initDiskDirManager()

	tmpDir := t.TempDir()
	testDir, _ := dm.GetPathProvider().Combine(tmpDir, "test_dir")
	dd, err := dm.CreateDir(testDir)
	assert.Nil(t, err)
	assert.Equal(t, "test_dir", dd.Name)
	assert.Equal(t, tmpDir+"/", dd.Path)

	fullPath, err := dd.FullPath()
	assert.Nil(t, err)
	assert.Equal(t, testDir, fullPath)
}

func TestDiskDirectoryManager_CreateDir_WhenExists(t *testing.T) {
	dm := initDiskDirManager()

	// Prepare dir...
	testDir, _ := dm.GetPathProvider().Combine(t.TempDir(), "test_dir")
	_, _ = dm.CreateDir(testDir)

	dd, err := dm.CreateDir(testDir)
	assert.Nil(t, dd)
	assert.NotNil(t, err)
	assert.True(t, os.IsExist(err))
}

func TestDiskDirectoryManager_GetDir_WhenNotExists(t *testing.T) {
	dm := initDiskDirManager()

	testDir, _ := dm.GetPathProvider().Combine(t.TempDir(), "test_dir")
	dd, err := dm.GetDir(testDir)
	assert.Nil(t, dd)
	assert.NotNil(t, err)
	assert.True(t, os.IsNotExist(err))
}

func TestDiskDirectoryManager_GetDir_WhenExists(t *testing.T) {
	dm := initDiskDirManager()

	// Prepare dir...
	tmpDir := t.TempDir()
	testDir, _ := dm.GetPathProvider().Combine(tmpDir, "test_dir")
	_, _ = dm.CreateDir(testDir)

	dd, err := dm.GetDir(testDir)
	assert.Nil(t, err)
	assert.Equal(t, "test_dir", dd.Name)
	assert.Equal(t, tmpDir+"/", dd.Path)

	fullPath, err := dd.FullPath()
	assert.Nil(t, err)
	assert.Equal(t, testDir, fullPath)
}

func TestDiskDirectoryManager_DirExists_WhenNonExists(t *testing.T) {
	dm := initDiskDirManager()

	testDir, _ := dm.GetPathProvider().Combine(t.TempDir(), "test_dir")
	exists, err := dm.DirExists(testDir)
	assert.Nil(t, err)
	assert.False(t, exists)
}

func TestDiskDirectoryManager_DirExists_WhenExists(t *testing.T) {
	dm := initDiskDirManager()

	// Prepare dir...
	testDir, _ := dm.GetPathProvider().Combine(t.TempDir(), "test_dir")
	_, _ = dm.CreateDir(testDir)

	exists, err := dm.DirExists(testDir)
	assert.Nil(t, err)
	assert.True(t, exists)
}

func TestDiskDirectoryManager_DeleteDir_WhenNotExists(t *testing.T) {
	dm := initDiskDirManager()

	testDir, _ := dm.GetPathProvider().Combine(t.TempDir(), "test_dir")
	err := dm.DeleteDir(testDir)
	assert.NotNil(t, err)
	assert.True(t, os.IsNotExist(err))
}

func TestDiskDirectoryManager_DeleteDir_WhenExists(t *testing.T) {
	dm := initDiskDirManager()

	// Prepare dir...
	testDir, _ := dm.GetPathProvider().Combine(t.TempDir(), "test_dir")
	_, _ = dm.CreateDir(testDir)

	err := dm.DeleteDir(testDir)
	assert.Nil(t, err)
}
