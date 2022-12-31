package filemanager

func createDir(path string, manager DirectoryManager) (DirectoryDetail, error) {
	return manager.createDir(path)
}

func getDir(path string, manager DirectoryManager) (DirectoryDetail, error) {
	return manager.getDir(path)
}

func dirExists(path string, manager DirectoryManager) (bool, error) {
	return manager.dirExists(path)
}

func deleteDir(path string, manager DirectoryManager) error {
	return manager.deleteDir(path)
}

func createFile(path string, manager FileManager) (FileDetail, error) {
	return manager.createFile(path)
}

func getFile(path string, manager FileManager) (FileDetail, error) {
	return manager.getFile(path)
}

func fileExists(path string, manager FileManager) (bool, error) {
	return manager.fileExists(path)
}

func deleteFile(path string, manager FileManager) error {
	return manager.deleteFile(path)
}
