using System.IO;
using BSTrueRandomizer.config;
using BSTrueRandomizer.Exceptions;
using BSTrueRandomizer.util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BSTrueRandomizerTest.config
{
    [TestClass]
    public class OptionsTest
    {
        private readonly Options _options = new Options();

        [TestInitialize]
        public void Setup()
        {
            // This is needed to avoid validation errors for UnrealPakPath in unrelated tests
            _options.IsJsonOnly = true;
        }

        [TestMethod]
        public void TestValidateEmptySeed()
        {
            _options.SeedText = "";

            Assert.ThrowsException<InputException>(() => _options.Validate());
        }

        [TestMethod]
        public void TestValidateBlankSpaceSeed()
        {
            _options.SeedText = "     ";

            Assert.ThrowsException<InputException>(() => _options.Validate());
        }

        [TestMethod]
        public void TestValidateNullSeed()
        {
            _options.SeedText = null;

            _options.Validate();
        }

        [TestMethod]
        public void TestValidateSingleCharacterSeed()
        {
            _options.SeedText = "i";

            _options.Validate();
        }

        [TestMethod]
        public void TestValidateStartingAndTrailingSpacesSeed()
        {
            _options.SeedText = " i ";

            _options.Validate();
        }

        [TestMethod]
        public void TestValidateMultipleWordsWithBlankSpacesSeed()
        {
            _options.SeedText = "words with spaces";

            _options.Validate();
        }

        [TestMethod]
        public void TestValidateNonAsciiSeed()
        {
            _options.SeedText = "シードのテキストが自由";

            _options.Validate();
        }

        [TestMethod]
        public void TestValidateNegativeKeyLocations()
        {
            _options.NumberOfKeyLocations = -1;

            Assert.ThrowsException<InputException>(() => _options.Validate());
        }

        [TestMethod]
        public void TestValidateZeroKeyLocations()
        {
            _options.NumberOfKeyLocations = 0;

            _options.Validate();
        }

        [TestMethod]
        public void TestValidatePositiveKeyLocations()
        {
            _options.NumberOfKeyLocations = 1;

            _options.Validate();
        }

        [TestMethod]
        public void TestValidateNonExistentOutputFolder()
        {
            _options.OutputPath = "non-existent/output/path";

            Assert.ThrowsException<InputException>(() => _options.Validate());
        }

        [TestMethod]
        public void TestValidateExistingOutputFolder()
        {
            _options.InputPath = Directory.GetCurrentDirectory();

            _options.Validate();
        }

        [TestMethod]
        public void TestValidateNonExistentInputFolder()
        {
            _options.InputPath = "non-existent/input/path";

            Assert.ThrowsException<InputException>(() => _options.Validate());
        }

        [TestMethod]
        public void TestValidateExistingInputFolder()
        {
            _options.InputPath = Directory.GetCurrentDirectory();

            _options.Validate();
        }

        [TestMethod]
        public void TestValidateNullInputFolder()
        {
            _options.InputPath = null;

            _options.Validate();
        }

        [TestMethod]
        public void TestValidateNonExistentUnrealPakFolder()
        {
            _options.UnrealPakPath = "non-existent/unrealpak/path";

            Assert.ThrowsException<InputException>(() => _options.Validate());
        }

        [TestMethod]
        public void TestValidateExistingUnrealPakFolder()
        {
            _options.UnrealPakPath = Directory.GetCurrentDirectory();

            _options.Validate();
        }

        [TestMethod]
        public void TestUserProvidedUnrealPakFolderWithMissingExecutable()
        {
            string providedEmptyExistingPakFolder = Path.Combine(Directory.GetCurrentDirectory(), "tmpUnrealPakTestFolder");
            Directory.CreateDirectory(providedEmptyExistingPakFolder);
            _options.IsJsonOnly = false;
            _options.UnrealPakPath = providedEmptyExistingPakFolder;

            try
            {
                Assert.ThrowsException<InputException>(() => _options.Validate());
            } finally
            {
                Directory.Delete(providedEmptyExistingPakFolder);
            }
        }

        [TestMethod]
        public void TestUserProvidedUnrealPakFolderWithMissingExecutableWhenDefaultIsNotEmpty()
        {
            string providedEmptyExistingPakFolder = Path.Combine(Directory.GetCurrentDirectory(), "tmpUnrealPakTestFolder");
            Directory.CreateDirectory(providedEmptyExistingPakFolder);
            _options.IsJsonOnly = false;
            _options.UnrealPakPath = providedEmptyExistingPakFolder;

            string dummyFilePath = Path.Combine(Directory.GetCurrentDirectory(), Constants.UnrealPakExeFileName);
            File.Create(dummyFilePath).Dispose();

            try
            {
                Assert.ThrowsException<InputException>(() => _options.Validate());
            }
            finally
            {
                Directory.Delete(providedEmptyExistingPakFolder);
                File.Delete(dummyFilePath);
            }
        }

        [TestMethod]
        public void TestUserProvidedUnrealPakFolderWithMissingExecutableWhenProvidedResourceIsNotEmpty()
        {
            string providedEmptyExistingPakFolder = Path.Combine(Directory.GetCurrentDirectory(), "tmpUnrealPakTestFolder");
            Directory.CreateDirectory(providedEmptyExistingPakFolder);
            _options.IsJsonOnly = false;
            _options.UnrealPakPath = providedEmptyExistingPakFolder;

            string dummyFilePath = Path.Combine(Directory.GetCurrentDirectory(), Constants.UnrealPakExeFileName);
            File.Create(dummyFilePath).Dispose();

            string pakExecutablePath = FileUtil.getResourcePath(Constants.UnrealPakResourcePath);
            DirectoryInfo resourceDir = Directory.GetParent(pakExecutablePath);
            bool isResourceExists = CreateProvidedResoucePakIfNotExists(pakExecutablePath, resourceDir);

            try
            {
                Assert.ThrowsException<InputException>(() => _options.Validate());
            }
            finally
            {
                Directory.Delete(providedEmptyExistingPakFolder);
                File.Delete(dummyFilePath);
                if (!isResourceExists)
                {
                    File.Delete(pakExecutablePath);
                    Directory.Delete(resourceDir.FullName);
                }
            }
        }

        [TestMethod]
        public void TestValidateDefaultUnrealPakFolderWithExistingExecutable()
        {
            _options.IsJsonOnly = false;
            string dummyFilePath = Path.Combine(_options.UnrealPakPath, Constants.UnrealPakExeFileName);
            File.Create(dummyFilePath).Dispose();

            try
            {
                _options.Validate();
            }
            finally
            {
                File.Delete(dummyFilePath);
            }
        }

        [TestMethod]
        public void TestValidateUnrealPakUserAndDefaultFolderEmptyProvidedResouceExists()
        {
            _options.IsJsonOnly = false;

            string pakExecutablePath = FileUtil.getResourcePath(Constants.UnrealPakResourcePath);
            DirectoryInfo resourceDir = Directory.GetParent(pakExecutablePath);
            bool isResourceExists = CreateProvidedResoucePakIfNotExists(pakExecutablePath, resourceDir);
            try
            {
                _options.Validate();
            }
            finally
            {
                if (!isResourceExists)
                {
                    File.Delete(pakExecutablePath);
                    Directory.Delete(resourceDir.FullName);
                }
            }
        }

        private static bool CreateProvidedResoucePakIfNotExists(string pakExecutablePath, DirectoryInfo resourceDir)
        {
            bool isResourceExists = File.Exists(pakExecutablePath);
            if (!isResourceExists)
            {
                Directory.CreateDirectory(resourceDir.FullName);
                File.Create(pakExecutablePath).Dispose();
            }

            return isResourceExists;
        }

        [TestMethod]
        public void TestNormalizeSeedRemovesLeadingAndTrailingSpaces()
        {
            _options.SeedText = "    UntrimmedSeed  ";

            _options.NormalizeInput();

            Assert.AreEqual("UntrimmedSeed", _options.SeedText);
        }

        [TestMethod]
        public void TestNormalizeSeedPreservesSpacesBetweenWords()
        {
            const string inputSeed = "Seed with spaces";
            _options.SeedText = inputSeed;

            _options.NormalizeInput();

            Assert.AreEqual(inputSeed, _options.SeedText);
        }

        [TestMethod]
        public void TestNormalizeNullSeedDoesNothing()
        {
            _options.SeedText = null;

            _options.NormalizeInput();

            Assert.IsNull(_options.SeedText);
        }

        [TestMethod]
        public void TestNormalizeOutputPathTrimsLeadingAndTrailingSpaces()
        {
            const string expectedOutput = "some/folder/path/";
            _options.OutputPath = "  " + expectedOutput + "    ";

            _options.NormalizeInput();

            Assert.AreEqual(expectedOutput, _options.OutputPath);
        }

        [TestMethod]
        public void TestNormalizeOutputPathAddsMissingTrailingSeparator()
        {
            const string providedPath = "some/folder/path";
            string expectedResultingPath = providedPath + Path.DirectorySeparatorChar;
            _options.OutputPath = providedPath;

            _options.NormalizeInput();

            Assert.AreEqual(expectedResultingPath, _options.OutputPath);
        }

        [TestMethod]
        public void TestNormalizeOutputPathRemovesLeadingAndTrailingQuotes()
        {
            const string basePath = "some\\folder\\path";
            const char quoteChar = '"';
            string providedPath = quoteChar + basePath + quoteChar;
            string expectedResultingPath = basePath + Path.DirectorySeparatorChar;
            _options.OutputPath = providedPath;

            _options.NormalizeInput();

            Assert.AreEqual(expectedResultingPath, _options.OutputPath);
        }

        [TestMethod]
        public void TestNormalizeInputPathTrimsLeadingAndTrailingSpaces()
        {
            const string expectedOutput = "some/folder/path/";
            _options.InputPath = "  " + expectedOutput + "    ";

            _options.NormalizeInput();

            Assert.AreEqual(expectedOutput, _options.InputPath);
        }

        [TestMethod]
        public void TestNormalizeInputPathAddsMissingTrailingSeparator()
        {
            const string providedPath = "some/folder/path";
            string expectedResultingPath = providedPath + Path.DirectorySeparatorChar;
            _options.InputPath = providedPath;

            _options.NormalizeInput();

            Assert.AreEqual(expectedResultingPath, _options.InputPath);
        }

        [TestMethod]
        public void TestNormalizeInputPathRemovesLeadingAndTrailingQuotes()
        {
            const string basePath = "some\\folder\\path";
            const char quoteChar = '"';
            string providedPath = quoteChar + basePath + quoteChar;
            string expectedResultingPath = basePath + Path.DirectorySeparatorChar;
            _options.InputPath = providedPath;

            _options.NormalizeInput();

            Assert.AreEqual(expectedResultingPath, _options.InputPath);
        }

        [TestMethod]
        public void TestNormalizeNullInputPathDoesNothing()
        {
            _options.InputPath = null;

            _options.NormalizeInput();

            Assert.IsNull(_options.InputPath);
        }
    }
}