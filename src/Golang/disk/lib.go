package disk

import (
	"fmt"
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
