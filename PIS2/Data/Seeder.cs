using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PIS2.Models;

namespace PIS2.Data
{
    public static class Seeder
    {
        // ============================================================
        // MAIN SEED METHOD
        // ============================================================

        public static async Task SeedRolesAndAdminAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<
                    RoleManager<IdentityRole<int>>>();

            var userManager =
                serviceProvider.GetRequiredService<
                    UserManager<userModel>>();

            var context =
                serviceProvider.GetRequiredService<PISContext>();

            // --------------------------------------------------------
            // ROLES
            // --------------------------------------------------------

            string[] roles =
            {
                "ADMIN",
                "HRMANAGER",
                "USER",
                "HRADMIN",
                "MANAGEMENT",
                "CLINIC",
                "HRPERSONNEL",
                "PAYROLL",
                "FINANCE"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole<int>
                        {
                            Name = role
                        });

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(
                            ", ",
                            result.Errors.Select(e => e.Description));

                        throw new Exception(
                            $"Failed to create role {role}: {errors}");
                    }
                }
            }

            // --------------------------------------------------------
            // ADMIN USER
            // --------------------------------------------------------

            var adminUser =
                await userManager.FindByNameAsync("admin");

            if (adminUser == null)
            {
                var newAdmin = new userModel
                {
                    UserName = "admin",
                    userStatus = Enums.mainStatus.Active,
                    modifiedBy = "System",
                    modifiedDate = DateTime.Now,
                    MustChangePassword = true
                };

                var result =
                    await userManager.CreateAsync(
                        newAdmin,
                        "123456.aA");

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description));

                    throw new Exception(
                        $"Failed to create admin user: {errors}");
                }

                var roleResult =
                    await userManager.AddToRoleAsync(
                        newAdmin,
                        "ADMIN");

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        roleResult.Errors.Select(e => e.Description));

                    throw new Exception(
                        $"Failed to assign ADMIN role: {errors}");
                }
            }

            // --------------------------------------------------------
            // LANGUAGES
            // --------------------------------------------------------

            await SeedLanguagesAsync(context);

            // --------------------------------------------------------
            // APPLICATION TRANSLATIONS
            // --------------------------------------------------------

            await SeedAppTranslationsAsync(context);
        }


        // ============================================================
        // LANGUAGES
        // ============================================================

        private static async Task SeedLanguagesAsync(
            PISContext context)
        {
            var languages = new List<LanguageModel>
            {
                new LanguageModel
                {
                    languageCode = "en",
                    languageName = "English",
                    nativeName = "English",
                    isRTL = false,
                    isActive = true,
                    isDefault = true
                },

                new LanguageModel
                {
                    languageCode = "am",
                    languageName = "Amharic",
                    nativeName = "አማርኛ",
                    isRTL = false,
                    isActive = true,
                    isDefault = false
                },

                new LanguageModel
                {
                    languageCode = "ti",
                    languageName = "Tigrinya",
                    nativeName = "ትግርኛ",
                    isRTL = false,
                    isActive = true,
                    isDefault = false
                }
            };

            foreach (var language in languages)
            {
                var existingLanguage =
                    await context.Languages
                        .FirstOrDefaultAsync(x =>
                            x.languageCode ==
                            language.languageCode);

                if (existingLanguage == null)
                {
                    context.Languages.Add(language);
                }
                else
                {
                    existingLanguage.languageName =
                        language.languageName;

                    existingLanguage.nativeName =
                        language.nativeName;

                    existingLanguage.isRTL =
                        language.isRTL;

                    existingLanguage.isActive =
                        language.isActive;

                    existingLanguage.isDefault =
                        language.isDefault;
                }
            }

            await context.SaveChangesAsync();
        }


        // ============================================================
        // APPLICATION TRANSLATIONS
        // ============================================================

        private static async Task SeedAppTranslationsAsync(
            PISContext context)
        {
            // --------------------------------------------------------
            // GET LANGUAGE IDs
            // --------------------------------------------------------

            var languages =
                await context.Languages
                    .AsNoTracking()
                    .Where(x =>
                        x.languageCode == "en" ||
                        x.languageCode == "am" ||
                        x.languageCode == "ti")
                    .ToDictionaryAsync(
                        x => x.languageCode,
                        x => x.languageID);

            if (!languages.ContainsKey("en") ||
                !languages.ContainsKey("am") ||
                !languages.ContainsKey("ti"))
            {
                throw new Exception(
                    "Required localization languages have not been seeded.");
            }

            // --------------------------------------------------------
            // TRANSLATION DEFINITIONS
            // --------------------------------------------------------

            var translations = new List<TranslationDefinition>
            {
                // ====================================================
                // COMMON
                // ====================================================

                new()
                {
                    Key = "Save",
                    English = "Save",
                    Amharic = "አስቀምጥ",
                    Tigrinya = "ዓቅብ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Cancel",
                    English = "Cancel",
                    Amharic = "ሰርዝ",
                    Tigrinya = "ሰርዝ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Close",
                    English = "Close",
                    Amharic = "ዝጋ",
                    Tigrinya = "ዕጸው",
                    Module = "Common"
                },

                new()
                {
                    Key = "Add",
                    English = "Add",
                    Amharic = "ጨምር",
                    Tigrinya = "ወስኽ",
                    Module = "Common"
                },

                new()
                {
                    Key = "New",
                    English = "New",
                    Amharic = "አዲስ",
                    Tigrinya = "ሓድሽ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Edit",
                    English = "Edit",
                    Amharic = "አርትዕ",
                    Tigrinya = "ኣርትዕ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Delete",
                    English = "Delete",
                    Amharic = "ሰርዝ",
                    Tigrinya = "ሰርዝ",
                    Module = "Common"
                },

                new()
                {
                    Key = "View",
                    English = "View",
                    Amharic = "ይመልከቱ",
                    Tigrinya = "ርአ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Details",
                    English = "Details",
                    Amharic = "ዝርዝር",
                    Tigrinya = "ዝርዝር",
                    Module = "Common"
                },

                new()
                {
                    Key = "Search",
                    English = "Search",
                    Amharic = "ፈልግ",
                    Tigrinya = "ድለ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Filter",
                    English = "Filter",
                    Amharic = "አጣራ",
                    Tigrinya = "ምረጽ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Reset",
                    English = "Reset",
                    Amharic = "ዳግም አስጀምር",
                    Tigrinya = "እንደገና ኣስጀምር",
                    Module = "Common"
                },

                new()
                {
                    Key = "Clear",
                    English = "Clear",
                    Amharic = "አጽዳ",
                    Tigrinya = "ኣጽርይ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Refresh",
                    English = "Refresh",
                    Amharic = "አድስ",
                    Tigrinya = "ሓድስ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Back",
                    English = "Back",
                    Amharic = "ተመለስ",
                    Tigrinya = "ተመለስ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Next",
                    English = "Next",
                    Amharic = "ቀጣይ",
                    Tigrinya = "ዝቕጽል",
                    Module = "Common"
                },

                new()
                {
                    Key = "Previous",
                    English = "Previous",
                    Amharic = "ቀዳማይ",
                    Tigrinya = "ዝሓለፈ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Yes",
                    English = "Yes",
                    Amharic = "አዎ",
                    Tigrinya = "እወ",
                    Module = "Common"
                },

                new()
                {
                    Key = "No",
                    English = "No",
                    Amharic = "አይ",
                    Tigrinya = "ኣይ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Confirm",
                    English = "Confirm",
                    Amharic = "አረጋግጥ",
                    Tigrinya = "ኣረጋግጽ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Actions",
                    English = "Actions",
                    Amharic = "ተግባራት",
                    Tigrinya = "ተግባራት",
                    Module = "Common"
                },

                new()
                {
                    Key = "Status",
                    English = "Status",
                    Amharic = "ሁኔታ",
                    Tigrinya = "ኩነታት",
                    Module = "Common"
                },

                new()
                {
                    Key = "Active",
                    English = "Active",
                    Amharic = "ንቁ",
                    Tigrinya = "ንጡፍ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Inactive",
                    English = "Inactive",
                    Amharic = "ንቁ ያልሆነ",
                    Tigrinya = "ንጡፍ ዘይኮነ",
                    Module = "Common"
                },


                // ====================================================
                // SYSTEM
                // ====================================================

                new()
                {
                    Key = "System",
                    English = "System",
                    Amharic = "ስርዓት",
                    Tigrinya = "ስርዓት",
                    Module = "System"
                },

                new()
                {
                    Key = "Settings",
                    English = "Settings",
                    Amharic = "ቅንብሮች",
                    Tigrinya = "ቅንብራት",
                    Module = "System"
                },

                new()
                {
                    Key = "Administration",
                    English = "Administration",
                    Amharic = "አስተዳደር",
                    Tigrinya = "ምምሕዳር",
                    Module = "System"
                },

                new()
                {
                    Key = "User",
                    English = "User",
                    Amharic = "ተጠቃሚ",
                    Tigrinya = "ተጠቃሚ",
                    Module = "System"
                },

                new()
                {
                    Key = "Users",
                    English = "Users",
                    Amharic = "ተጠቃሚዎች",
                    Tigrinya = "ተጠቀምቲ",
                    Module = "System"
                },

                new()
                {
                    Key = "Role",
                    English = "Role",
                    Amharic = "ሚና",
                    Tigrinya = "ተራ",
                    Module = "System"
                },

                new()
                {
                    Key = "Roles",
                    English = "Roles",
                    Amharic = "ሚናዎች",
                    Tigrinya = "ተራታት",
                    Module = "System"
                },

                new()
                {
                    Key = "Permission",
                    English = "Permission",
                    Amharic = "ፈቃድ",
                    Tigrinya = "ፍቓድ",
                    Module = "System"
                },

                new()
                {
                    Key = "Permissions",
                    English = "Permissions",
                    Amharic = "ፈቃዶች",
                    Tigrinya = "ፍቓዳት",
                    Module = "System"
                },

                new()
                {
                    Key = "Language",
                    English = "Language",
                    Amharic = "ቋንቋ",
                    Tigrinya = "ቋንቋ",
                    Module = "System"
                },

                new()
                {
                    Key = "Languages",
                    English = "Languages",
                    Amharic = "ቋንቋዎች",
                    Tigrinya = "ቋንቋታት",
                    Module = "System"
                },

                new()
                {
                    Key = "Login",
                    English = "Login",
                    Amharic = "ግባ",
                    Tigrinya = "እቶ",
                    Module = "System"
                },

                new()
                {
                    Key = "Logout",
                    English = "Logout",
                    Amharic = "ውጣ",
                    Tigrinya = "ውጻእ",
                    Module = "System"
                },

                new()
                {
                    Key = "Password",
                    English = "Password",
                    Amharic = "የይለፍ ቃል",
                    Tigrinya = "መሕለፊ ቃል",
                    Module = "System"
                },

                new()
                {
                    Key = "ChangePassword",
                    English = "Change Password",
                    Amharic = "የይለፍ ቃል ቀይር",
                    Tigrinya = "መሕለፊ ቃል ቀይር",
                    Module = "System"
                },


                // ====================================================
                // APPLICATION
                // ====================================================

                new()
                {
                    Key = "Dashboard",
                    English = "Dashboard",
                    Amharic = "ዳሽቦርድ",
                    Tigrinya = "ዳሽቦርድ",
                    Module = "Application"
                },

                new()
                {
                    Key = "Home",
                    English = "Home",
                    Amharic = "መነሻ",
                    Tigrinya = "መበገሲ",
                    Module = "Application"
                },

                new()
                {
                    Key = "Employee",
                    English = "Employee",
                    Amharic = "ሰራተኛ",
                    Tigrinya = "ሰራሕተኛ",
                    Module = "Application"
                },

                new()
                {
                    Key = "Employees",
                    English = "Employees",
                    Amharic = "ሰራተኞች",
                    Tigrinya = "ሰራሕተኛታት",
                    Module = "Application"
                },

                new()
                {
                    Key = "Department",
                    English = "Department",
                    Amharic = "ክፍል",
                    Tigrinya = "ክፍሊ",
                    Module = "Application"
                },

                new()
                {
                    Key = "Departments",
                    English = "Departments",
                    Amharic = "ክፍሎች",
                    Tigrinya = "ክፍልታት",
                    Module = "Application"
                },

                new()
                {
                    Key = "Company",
                    English = "Company",
                    Amharic = "ድርጅት",
                    Tigrinya = "ትካል",
                    Module = "Application"
                },

                new()
                {
                    Key = "Companies",
                    English = "Companies",
                    Amharic = "ድርጅቶች",
                    Tigrinya = "ትካላት",
                    Module = "Application"
                },

                new()
                {
                    Key = "Position",
                    English = "Position",
                    Amharic = "የስራ መደብ",
                    Tigrinya = "መደብ ስራሕ",
                    Module = "Application"
                },

                new()
                {
                    Key = "Positions",
                    English = "Positions",
                    Amharic = "የስራ መደቦች",
                    Tigrinya = "መደባት ስራሕ",
                    Module = "Application"
                },

                new()
                {
                    Key = "Report",
                    English = "Report",
                    Amharic = "ሪፖርት",
                    Tigrinya = "ጸብጻብ",
                    Module = "Application"
                },

                new()
                {
                    Key = "Reports",
                    English = "Reports",
                    Amharic = "ሪፖርቶች",
                    Tigrinya = "ጸብጻባት",
                    Module = "Application"
                },

                new()
                {
                    Key = "Date",
                    English = "Date",
                    Amharic = "ቀን",
                    Tigrinya = "ዕለት",
                    Module = "Common"
                },

                new()
                {
                    Key = "Description",
                    English = "Description",
                    Amharic = "መግለጫ",
                    Tigrinya = "መግለጺ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Name",
                    English = "Name",
                    Amharic = "ስም",
                    Tigrinya = "ስም",
                    Module = "Common"
                },

                new()
                {
                    Key = "Code",
                    English = "Code",
                    Amharic = "ኮድ",
                    Tigrinya = "ኮድ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Type",
                    English = "Type",
                    Amharic = "አይነት",
                    Tigrinya = "ዓይነት",
                    Module = "Common"
                },

                new()
                {
                    Key = "Required",
                    English = "Required",
                    Amharic = "አስፈላጊ",
                    Tigrinya = "ኣድላዪ",
                    Module = "Common"
                },

                new()
                {
                    Key = "Optional",
                    English = "Optional",
                    Amharic = "አማራጭ",
                    Tigrinya = "ኣማራጺ",
                    Module = "Common"
                },


                // ====================================================
                // MESSAGES
                // ====================================================

                new()
                {
                    Key = "SavedSuccessfully",
                    English = "Saved successfully.",
                    Amharic = "በተሳካ ሁኔታ ተቀምጧል።",
                    Tigrinya = "ብዓወት ተዓቂቡ።",
                    Module = "Messages"
                },

                new()
                {
                    Key = "UpdatedSuccessfully",
                    English = "Updated successfully.",
                    Amharic = "በተሳካ ሁኔታ ተሻሽሏል።",
                    Tigrinya = "ብዓወት ተሓዲሱ።",
                    Module = "Messages"
                },

                new()
                {
                    Key = "DeletedSuccessfully",
                    English = "Deleted successfully.",
                    Amharic = "በተሳካ ሁኔታ ተሰርዟል።",
                    Tigrinya = "ብዓወት ተሰሪዙ።",
                    Module = "Messages"
                },

                new()
                {
                    Key = "DeleteConfirmation",
                    English = "Are you sure you want to delete this record?",
                    Amharic = "ይህን መዝገብ ማጥፋት እርግጠኛ ነዎት?",
                    Tigrinya = "ነዚ መዝገብ ክትስርዞ ርግጸኛ ዲኻ?",
                    Module = "Messages"
                },

                new()
                {
                    Key = "NoRecordsFound",
                    English = "No records found.",
                    Amharic = "ምንም መዝገብ አልተገኘም።",
                    Tigrinya = "ዝኾነ መዝገብ ኣይተረኽበን።",
                    Module = "Messages"
                },

                new()
                {
                    Key = "ErrorOccurred",
                    English = "An error occurred.",
                    Amharic = "ስህተት ተከስቷል።",
                    Tigrinya = "ጌጋ ተፈጢሩ።",
                    Module = "Messages"
                },

                new()
                {
                    Key = "OperationSuccessful",
                    English = "Operation completed successfully.",
                    Amharic = "ክዋኔው በተሳካ ሁኔታ ተጠናቋል።",
                    Tigrinya = "እቲ ክዋነ ብዓወት ተዛዚሙ።",
                    Module = "Messages"
                }
            };


            // --------------------------------------------------------
            // INSERT / UPDATE TRANSLATIONS
            // --------------------------------------------------------

            foreach (var item in translations)
            {
                await UpsertTranslationAsync(
                    context,
                    item,
                    languages);
            }

            await context.SaveChangesAsync();
        }


        // ============================================================
        // UPSERT TRANSLATION
        // ============================================================

        private static async Task UpsertTranslationAsync(
            PISContext context,
            TranslationDefinition definition,
            Dictionary<string, int> languages)
        {
            var values = new Dictionary<string, string>
            {
                ["en"] = definition.English,
                ["am"] = definition.Amharic,
                ["ti"] = definition.Tigrinya
            };

            foreach (var language in values)
            {
                var languageID =
                    languages[language.Key];

                var existing =
                    await context.AppTranslations
                        .FirstOrDefaultAsync(x =>
                            x.translationKey ==
                            definition.Key &&
                            x.languageID ==
                            languageID);

                if (existing == null)
                {
                    context.AppTranslations.Add(
                        new AppTranslationModel
                        {
                            translationKey =
                                definition.Key,

                            languageID =
                                languageID,

                            translationValue =
                                language.Value,

                            module =
                                definition.Module,

                            description =
                                definition.Description,

                            isActive = true
                        });
                }
                else
                {
                    existing.translationValue =
                        language.Value;

                    existing.module =
                        definition.Module;

                    existing.description =
                        definition.Description;

                    existing.isActive = true;
                }
            }
        }


        // ============================================================
        // TRANSLATION DEFINITION
        // ============================================================

        private class TranslationDefinition
        {
            public string Key { get; set; } = string.Empty;

            public string English { get; set; } = string.Empty;

            public string Amharic { get; set; } = string.Empty;

            public string Tigrinya { get; set; } = string.Empty;

            public string? Module { get; set; }

            public string? Description { get; set; }
        }
    }
}