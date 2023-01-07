package filemanager

type FileDetail struct {
	fileManager FileManager
	Name        string
	Path        string
	Length      int64
}

func InitFileDetail(fm FileManager) FileDetail {
	return FileDetail{
		fileManager: fm,
	}
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

func InitDirectoryDetail(dm DirectoryManager) DirectoryDetail {
	return DirectoryDetail{
		dirManager: dm,
	}
}

func (d *DirectoryDetail) FullPath() (string, error) {
	return d.dirManager.GetPathProvider().Combine(
		d.Path, d.Name,
	)
}
