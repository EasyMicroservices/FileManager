package disk

import (
	"fmt"
	fm "github.com/EasyMicroservices/FileManager"
	"os"
	"path/filepath"
	"strings"
)

type SystemPathProvider struct {
}

func (s *SystemPathProvider) Combine(paths ...string) (string, error) {
	return filepath.Join(paths...), nil
}

func normalizePath(path string) string {
	for strings.HasSuffix(path, "/") {
		path = strings.TrimSuffix(path, "/")
	}

	return path
}

func (s *SystemPathProvider) GetObjectName(path string) (string, error) {
	path = normalizePath(path)
	path, err := s.Combine(path)
	if err != nil {
		return "", err
	}

	_, file := filepath.Split(path)

	if file != "" {
		return file, nil
	}

	return "", fmt.Errorf("invalid path: %v", path)
}

func (s *SystemPathProvider) GetObjectParentPath(path string) (string, error) {
	path = normalizePath(path)
	path, err := s.Combine(path)
	if err != nil {
		return "", err
	}

	dir, _ := filepath.Split(path)

	if dir != "" {
		return dir, nil
	}

	return "", fmt.Errorf("invalid path: %v", path)
}

type DiskFileManager struct {
	pathProvider fm.PathProvider
	dirManager   fm.DirectoryManager
}

func InitDiskFileManager(pathProvider fm.PathProvider, dirManager fm.DirectoryManager) DiskFileManager {
	return DiskFileManager{
		pathProvider: pathProvider,
		dirManager:   dirManager,
	}
}

func (d *DiskFileManager) GetPathProvider() fm.PathProvider {
	return d.pathProvider
}

func (d *DiskFileManager) GetDirectoryManager() fm.DirectoryManager {
	return d.dirManager
}

func (d *DiskFileManager) CreateFile(path string) (*fm.FileDetail, error) {
	exists, err := d.FileExists(path)
	if err != nil {
		return nil, err
	}

	if exists {
		return nil, os.ErrExist
	}

	path, _ = d.GetPathProvider().Combine(path)

	_, err = os.Create(path)
	if err != nil {
		return nil, err
	}

	return d.GetFile(path)
}

func (d *DiskFileManager) GetFile(path string) (*fm.FileDetail, error) {
	path, err := d.GetPathProvider().Combine(path)
	if err != nil {
		return nil, err
	}

	fileInfo, err := os.Stat(path)
	if err != nil {
		return nil, err
	}

	if fileInfo.IsDir() {
		return nil, os.ErrInvalid
	}

	fd := fm.InitFileDetail(d)
	fd.Name = fileInfo.Name()
	fd.Length = fileInfo.Size()
	fd.Path, _ = d.GetPathProvider().GetObjectParentPath(path)
	return &fd, nil
}

func (d *DiskFileManager) FileExists(path string) (bool, error) {
	path, err := d.GetPathProvider().Combine(path)
	if err != nil {
		return false, err
	}
	_, err = os.Stat(path)

	if os.IsNotExist(err) {
		return false, nil
	}

	if err != nil {
		return false, err
	}
	return true, nil
}

func (d *DiskFileManager) DeleteFile(path string) error {
	exists, err := d.FileExists(path)

	if err != nil {
		return err
	}

	if !exists {
		return os.ErrNotExist
	}

	path, _ = d.GetPathProvider().Combine(path)

	return os.RemoveAll(path)
}

type DiskDirectoryManager struct {
	pathProvider fm.PathProvider
}

func InitDiskDirectoryManager(pathProvider fm.PathProvider) DiskDirectoryManager {
	return DiskDirectoryManager{
		pathProvider: pathProvider,
	}
}

func (d DiskDirectoryManager) GetPathProvider() fm.PathProvider {
	return d.pathProvider
}

func (d DiskDirectoryManager) CreateDir(path string) (*fm.DirectoryDetail, error) {
	path, err := d.GetPathProvider().Combine(path)

	if err != nil {
		return nil, err
	}

	err = os.Mkdir(path, 0777)

	if err != nil {
		return nil, err
	}

	return d.GetDir(path)
}

func (d DiskDirectoryManager) GetDir(path string) (*fm.DirectoryDetail, error) {
	path, err := d.GetPathProvider().Combine(path)

	if err != nil {
		return nil, err
	}
	dirInfo, err := os.Stat(path)

	if err != nil {
		return nil, err
	}

	if !dirInfo.IsDir() {
		return nil, os.ErrInvalid
	}

	dd := fm.InitDirectoryDetail(d)
	dd.Name = dirInfo.Name()
	dd.Path, _ = d.GetPathProvider().GetObjectParentPath(path)

	return &dd, nil
}

func (d DiskDirectoryManager) DirExists(path string) (bool, error) {
	path, err := d.GetPathProvider().Combine(path)
	if err != nil {
		return false, err
	}
	_, err = os.Stat(path)

	if os.IsNotExist(err) {
		return false, nil
	}

	if err != nil {
		return false, err
	}
	return true, nil
}

func (d DiskDirectoryManager) DeleteDir(path string) error {
	exists, err := d.DirExists(path)

	if err != nil {
		return err
	}

	if !exists {
		return os.ErrNotExist
	}

	path, _ = d.GetPathProvider().Combine(path)

	return os.RemoveAll(path)
}
