﻿namespace System.IO.Abstractions.TestingHelpers.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using XFS = MockUnixSupport;

    [TestFixture]
    public class MockFileStreamTests
    {
        [Test]
        public void MockFileStream_Flush_WritesByteToFile()
        {
            // Arrange
            var filepath = XFS.Path(@"c:\something\foo.txt");
            var filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var cut = new MockFileStream(filesystem, filepath, MockFileStream.StreamType.WRITE);

            // Act
            cut.WriteByte(255);
            cut.Flush();

            // Assert
            CollectionAssert.AreEqual(new byte[]{255}, filesystem.GetFile(filepath).Contents);
        }

        [Test]
        public void MockFileStream_Dispose_ShouldNotResurrectFile()
        {
            var fileSystem = new MockFileSystem();
            var path = XFS.Path("C:\\test");
            var directory = fileSystem.Path.GetDirectoryName(path);
            fileSystem.AddFile(path, new MockFileData("Bla"));
            var stream = fileSystem.File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete);

            var fileCount1 = fileSystem.Directory.GetFiles(directory, "*").Length;
            fileSystem.File.Delete(path);
            var fileCount2 = fileSystem.Directory.GetFiles(directory, "*").Length;
            stream.Dispose();
            var fileCount3 = fileSystem.Directory.GetFiles(directory, "*").Length;

            Assert.AreEqual(1, fileCount1, "File should have existed");
            Assert.AreEqual(0, fileCount2, "File should have been deleted");
            Assert.AreEqual(0, fileCount3, "Disposing stream should not have resurrected the file");
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void MockFileStream_Constructor_Reading_Nonexistent_File_Throws_Exception()
        {
            // Arrange
            var nonexistentFilePath = XFS.Path(@"c:\something\foo.txt");
            var filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());         

            // Act
            var illegalFileStream = new MockFileStream(filesystem, nonexistentFilePath, MockFileStream.StreamType.READ);

            // Assert - expect an exception
        }
    }
}
