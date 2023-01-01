package disk

import (
	"github.com/stretchr/testify/assert"
	"testing"
)

func TestCombine(t *testing.T) {
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

func TestGetObjectName(t *testing.T) {
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

func TestGetObjectParentPath(t *testing.T) {
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
