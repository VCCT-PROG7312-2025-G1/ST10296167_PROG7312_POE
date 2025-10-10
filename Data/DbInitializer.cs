using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Data
{
    public static class DbInitializer
    {
        // This method initializes and seeds the database with default roles, users, and demo data.
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if db is seeded
                if (context.Roles.Any() && context.Issues.Any())
                {
                    return;
                }

                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Create roles if they don't exist
                if (!context.Roles.Any())
                {
                    string[] roleNames = { "Employee", "Admin" };
                    foreach (var roleName in roleNames)
                    {
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            await roleManager.CreateAsync(new IdentityRole(roleName));
                        }
                    }
                }

                // Create default admin user if it doesn't exist
                var adminEmail = "admin@municipal.gov.za";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = "Admin",
                        LastName = "User"
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        Console.WriteLine("Created ADMIN");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine(error.Description);
                        }
                    }
                }

                // Create a test employee
                var employeeEmail = "employee@municipal.gov.za";
                if (await userManager.FindByEmailAsync(employeeEmail) == null)
                {
                    var employeeUser = new User
                    {
                        UserName = employeeEmail,
                        Email = employeeEmail,
                        EmailConfirmed = true,
                        FirstName = "John",
                        LastName = "Doe"
                    };

                    var result = await userManager.CreateAsync(employeeUser, "Employee123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(employeeUser, "Employee");
                        Console.WriteLine("Created EMPLOYEE");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine(error.Description);
                        }
                    }
                }

                // Seed demo data if database is empty
                await SeedDemoDataAsync(context);
            }
        }

        private static async Task SeedDemoDataAsync(ApplicationDbContext context)
        {
            // Check if we already have data
            if (await context.Issues.AnyAsync())
            {
                Console.WriteLine("Database already contains data. Skipping demo data seeding.");
                return;
            }

            Console.WriteLine("Seeding database with demo data...");

            try
            {
                // Create demo issues
                var demoIssues = new List<Issue>
                {
                    new Issue
                    {
                        Category = "Water & Sanitation",
                        Description = "Large pothole causing damage to vehicles and creating safety hazards for pedestrians.",
                        Address = "123 Main Street",
                        Suburb = "Hillcrest",
                        Location = "123 Main Street, Hillcrest",
                        CreatedAt = DateTime.Now.AddDays(-5)
                    },
                    new Issue
                    {
                        Category = "Roads & Transport",
                        Description = "Streetlight has been out for over two weeks, creating unsafe conditions at night.",
                        Address = "456 Oak Avenue",
                        Suburb = "Westville",
                        Location = "456 Oak Avenue, Westville",
                        CreatedAt = DateTime.Now.AddDays(-3)
                    },
                    new Issue
                    {
                        Category = "Public Safety",
                        Description = "Burst water pipe flooding the street and causing water wastage.",
                        Address = "789 Pine Road",
                        Suburb = "Durban North",
                        Location = "789 Pine Road, Durban North",
                        CreatedAt = DateTime.Now.AddDays(-2)
                    },
                    new Issue
                    {
                        Category = "Waste Management",
                        Description = "Illegal dumping site with household waste and construction debris.",
                        Address = "321 Cedar Street",
                        Suburb = "Pinetown",
                        Location = "321 Cedar Street, Pinetown",
                        CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    new Issue
                    {
                        Category = "Parks & Recreation",
                        Description = "Playground equipment is damaged and unsafe for children to use.",
                        Address = "654 Maple Drive",
                        Suburb = "Kloof",
                        Location = "654 Maple Drive, Kloof",
                        CreatedAt = DateTime.Now.AddHours(-6)
                    }
                };

                // Add issues to database
                context.Issues.AddRange(demoIssues);
                await context.SaveChangesAsync();

                // Create demo files for the issues
                var demoFiles = new List<UploadedFile>
                {
                    new UploadedFile
                    {
                        FileName = "pothole1.jpg",
                        MimeType = "image/jpeg",
                        Size = 1024000,
                        FilePath = await CopySampleFileToUploads("pothole1.jpg", "sample/images/pothole1.jpg"),
                        IssueID = demoIssues[0].ID,
                        CreatedAt = DateTime.Now.AddDays(-5)
                    },
                    new UploadedFile
                    {
                        FileName = "pothole2.jpg",
                        MimeType = "image/jpeg",
                        Size = 856000,
                        FilePath = await CopySampleFileToUploads("pothole2.jpg", "sample/images/pothole2.jpg"),
                        IssueID = demoIssues[0].ID,
                        CreatedAt = DateTime.Now.AddDays(-5)
                    },
                    new UploadedFile
                    {
                        FileName = "streetlight.jpg",
                        MimeType = "image/jpeg",
                        Size = 1200000,
                        FilePath = await CopySampleFileToUploads("streetlight.jpg", "sample/images/streetlight.jpeg"),
                        IssueID = demoIssues[1].ID,
                        CreatedAt = DateTime.Now.AddDays(-3)
                    },
                    new UploadedFile
                    {
                        FileName = "waterpipe.jpg",
                        MimeType = "image/jpeg",
                        Size = 980000,
                        FilePath = await CopySampleFileToUploads("waterpipe.jpg", "sample/images/waterpipe.jpg"),
                        IssueID = demoIssues[2].ID,
                        CreatedAt = DateTime.Now.AddDays(-2)
                    },
                    new UploadedFile
                    {
                        FileName = "dumping.jpg",
                        MimeType = "image/jpeg",
                        Size = 1150000,
                        FilePath = await CopySampleFileToUploads("dumping.jpg", "sample/images/dumping.jpg"),
                        IssueID = demoIssues[3].ID,
                        CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    new UploadedFile
                    {
                        FileName = "playground.jpg",
                        MimeType = "image/jpeg",
                        Size = 890000,
                        FilePath = await CopySampleFileToUploads("playground.jpg", "sample/images/playground.jpeg"),
                        IssueID = demoIssues[4].ID,
                        CreatedAt = DateTime.Now.AddHours(-6)
                    }
                };

                // Add files to database
                context.UploadedFiles.AddRange(demoFiles);
                await context.SaveChangesAsync();

                // Create demo feedback
                var demoFeedback = new List<Feedback>
                {
                    new Feedback
                    {
                        Rating = 5,
                        OptionalFeedback = "Great service! The issue was resolved quickly.",
                        CreatedAt = DateTime.Now.AddDays(-4)
                    },
                    new Feedback
                    {
                        Rating = 4,
                        OptionalFeedback = "Good response time, but could be faster.",
                        CreatedAt = DateTime.Now.AddDays(-2)
                    },
                    new Feedback
                    {
                        Rating = 5,
                        OptionalFeedback = "Excellent work! Very professional.",
                        CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    new Feedback
                    {
                        Rating = 3,
                        OptionalFeedback = "Average service. Could improve communication.",
                        CreatedAt = DateTime.Now.AddHours(-12)
                    },
                    new Feedback
                    {
                        Rating = 4,
                        OptionalFeedback = "Good overall experience.",
                        CreatedAt = DateTime.Now.AddHours(-3)
                    }
                };

                // Add feedback to database
                context.Feedbacks.AddRange(demoFeedback);
                await context.SaveChangesAsync();

                Console.WriteLine($"Demo data seeded successfully: {demoIssues.Count} issues, {demoFiles.Count} files, {demoFeedback.Count} feedback entries.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding demo data: {ex.Message}");
            }
        }

        private static async Task<string> CopySampleFileToUploads(string fileName, string wwwrootRelativePath)
        {
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string wwwrootPath = Path.Combine(currentDirectory, "wwwroot");
                string sourceFilePath = Path.Combine(wwwrootPath, wwwrootRelativePath.Replace('/', Path.DirectorySeparatorChar));
                string uploadsFolder = Path.Combine(currentDirectory, "wwwroot", "uploads");
                
                // Ensure uploads directory exists
                Directory.CreateDirectory(uploadsFolder);

                // Check if source file exists
                if (!File.Exists(sourceFilePath))
                {
                    Console.WriteLine($"Sample file not found: {sourceFilePath}");
                    return string.Empty;
                }

                string uniqueFileName = Guid.NewGuid() + Path.GetExtension(fileName);
                string destinationPath = Path.Combine(uploadsFolder, uniqueFileName);

                // Copy file to uploads directory
                File.Copy(sourceFilePath, destinationPath);

                Console.WriteLine($"Copied sample file: {fileName} -> {destinationPath}");
                return destinationPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying file {fileName}: {ex.Message}");
                return string.Empty;
            }
        }
    }
}