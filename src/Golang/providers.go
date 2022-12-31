package filemanager

type PathProvider interface {
	combine(paths ...string) (string, error)
	getObjectName(path string) (string, error)
	getObjectParentPath(path string) (string, error)
}

type DirectoryManager interface {
	getPathProvider() PathProvider
	createDir(path string) (DirectoryDetail, error)
	getDir(path string) (DirectoryDetail, error)
	dirExists(path string) (bool, error)
	deleteDir(path string) error
}

type FileManager interface {
	getPathProvider() PathProvider
	getDirectoryManager() DirectoryManager
	createFile(path string) (FileDetail, error)
	getFile(path string) (FileDetail, error)
	fileExists(path string) (bool, error)
	deleteFile(path string) error
}
