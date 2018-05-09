
/*
 * Creating root source code distribution folder, that
 * will later be zipped, after copying of all files is done.
 */
CreateDirectory("./phosphorusfive-8-1");

/*
 * Copying all source code file.
 *
 * We copy Hyperlambda files, C# files, and all (relevant) ASP.NET files.
 */
var files = GetFiles ("./**/*.cs$|*.hl$|*.md$|*.aspx$|*.ascx$|*.sh$|*.cake$|*.sln$|*.csproj$|*.asax$|*.ico$||*.css$|*.js$|*.png$|*.jpg$|*.jpeg$|*.gif$|*.svg$|LICENSE$|*.nupkg$|*.config$|*.targets$|*.txt$");
CopyFiles (files, "phosphorusfive-8-1", true);
files = GetFiles ("./packages/**/*");
CopyFiles (files, "phosphorusfive-8-1/packages/", true);

/*
 * Deleting "auth.hl" file.
 */
DeleteFile ("./phosphorusfive-8-1/core/p5.webapp/auth.hl");

/*
 * Deleting "/db/" folder.
 */
var directoriesToDelete = new DirectoryPath [] { Directory("./phosphorusfive-8-1/core/p5.webapp/db/")};
DeleteDirectories (directoriesToDelete, new DeleteDirectorySettings {Recursive = true, Force = true});

/*
 * Deleting "common" folder.
 */
directoriesToDelete = new DirectoryPath [] { Directory("./phosphorusfive-8-1/core/p5.webapp/common/")};
DeleteDirectories (directoriesToDelete, new DeleteDirectorySettings {Recursive = true, Force = true});

/*
 * Deleting "users" folders.
 */
directoriesToDelete = new DirectoryPath [] { Directory("./phosphorusfive-8-1/core/p5.webapp/users/")};
DeleteDirectories (directoriesToDelete, new DeleteDirectorySettings {Recursive = true, Force = true});

/*
 * Then re-creating the "/common/" folder that were removed above.
 *
 * This is done to avoid copying temporary files, created during usage of system.
 */
CreateDirectory ("./phosphorusfive-8-1/core/p5.webapp/common/");
CreateDirectory ("./phosphorusfive-8-1/core/p5.webapp/common/documents/");
CreateDirectory ("./phosphorusfive-8-1/core/p5.webapp/common/documents/private/");
CreateDirectory ("./phosphorusfive-8-1/core/p5.webapp/common/documents/public/");
CreateDirectory ("./phosphorusfive-8-1/core/p5.webapp/common/temp/");

files = GetFiles ("./core/p5.webapp/common/README.md");
CopyFiles (files, "./phosphorusfive-8-1/core/p5.webapp/common/");

files = GetFiles ("./core/p5.webapp/common/documents/private/README.md");
CopyFiles (files, "./phosphorusfive-8-1/core/p5.webapp/common/documents/private/");

files = GetFiles ("./core/p5.webapp/common/documents/public/README.md");
CopyFiles (files, "./phosphorusfive-8-1/core/p5.webapp/common/documents/public/");

files = GetFiles ("./core/p5.webapp/common/temp/README.md");
CopyFiles (files, "./phosphorusfive-8-1/core/p5.webapp/common/temp/");


/*
 * Then re-creating the "/users/root/" folder that were removed above.
 *
 * This is done to avoid copying temporary files, created during usage of system.
 */
CreateDirectory ("./phosphorusfive-8-1/core/p5.webapp/users/root/");
CreateDirectory ("./phosphorusfive-8-1/core/p5.webapp/users/root/documents/");
CreateDirectory ("./phosphorusfive-8-1/core/p5.webapp/users/root/documents/private/");
CreateDirectory ("./phosphorusfive-8-1/core/p5.webapp/users/root/documents/public/");
CreateDirectory ("./phosphorusfive-8-1/core/p5.webapp/users/root/temp/");

files = GetFiles ("./core/p5.webapp/users/README.md");
CopyFiles (files, "./phosphorusfive-8-1/core/p5.webapp/users/");

files = GetFiles ("./core/p5.webapp/users/root/documents/private/README.md");
CopyFiles (files, "./phosphorusfive-8-1/core/p5.webapp/users/root/documents/private/");

files = GetFiles ("./core/p5.webapp/users/root/documents/public/README.md");
CopyFiles (files, "./phosphorusfive-8-1/core/p5.webapp/users/root/documents/public/");

files = GetFiles ("./core/p5.webapp/users/root/temp/README.md");
CopyFiles (files, "./phosphorusfive-8-1/core/p5.webapp/users/root/temp/");

/*
 * Zipping folder, to create our actual distribution.
 */
Zip ("./phosphorusfive-8-1", "./Source-Complete-With-Submodules.zip");

/*
 * Deleting temporary folder.
 */
directoriesToDelete = new DirectoryPath [] { Directory("./phosphorusfive-8-1/")};
DeleteDirectories (directoriesToDelete, new DeleteDirectorySettings {Recursive = true, Force = true});

