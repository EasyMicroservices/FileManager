package filemanager

type PathProvider interface {
	Combine(paths ...string) (string, error)
	GetObjectName(path string) (string, error)
	GetObjectParentPath(path string) (string, error)
}

type DirectoryManager interface {
	GetPathProvider() PathProvider
	CreateDir(path string) (*DirectoryDetail, error)
	GetDir(path string) (*DirectoryDetail, error)
	DirExists(path string) (bool, error)
	DeleteDir(path string) error
}

type FileManager interface {
	GetPathProvider() PathProvider
	GetDirectoryManager() DirectoryManager
	CreateFile(path string) (*FileDetail, error)
	GetFile(path string) (*FileDetail, error)
	FileExists(path string) (bool, error)
	DeleteFile(path string) error
}
