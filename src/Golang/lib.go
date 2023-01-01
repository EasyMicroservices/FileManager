package filemanager

func createDir(path string, manager DirectoryManager) (DirectoryDetail, error) {
	return manager.CreateDir(path)
}

func getDir(path string, manager DirectoryManager) (DirectoryDetail, error) {
	return manager.GetDir(path)
}

func dirExists(path string, manager DirectoryManager) (bool, error) {
	return manager.DirExists(path)
}

func deleteDir(path string, manager DirectoryManager) error {
	return manager.DeleteDir(path)
}

func createFile(path string, manager FileManager) (FileDetail, error) {
	return manager.CreateFile(path)
}

func getFile(path string, manager FileManager) (FileDetail, error) {
	return manager.GetFile(path)
}

func fileExists(path string, manager FileManager) (bool, error) {
	return manager.FileExists(path)
}

func deleteFile(path string, manager FileManager) error {
	return manager.DeleteFile(path)
}
