package filemanager

type FileDetail struct {
	fileManager FileManager
	Name        string
	Path        string
	Length      int64
}

func (f *FileDetail) FullPath() (string, error) {
	return f.fileManager.GetPathProvider().Combine(
		f.Path, f.Name,
	)
}

type DirectoryDetail struct {
	dirManager DirectoryManager
	Name       string
	Path       string
}

func (d *DirectoryDetail) FullPath() (string, error) {
	return d.dirManager.GetPathProvider().Combine(
		d.Path, d.Name,
	)
}
