using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Data
{
    public static class DbInitializer
    {
        // This method initializes and seeds the database with default roles, users, and demo data (used ChatGPT to generate & seed demo data).
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

                // Seed demo data (each type has its own conditional check inside)
                await SeedDemoDataAsync(context);
            }
        }

        private static async Task SeedDemoDataAsync(ApplicationDbContext context)
        {
            Console.WriteLine("Seeding database with demo data (individual checks per type)...");

            try
            {
                // Issues
                await SeedIssuesAsync(context);

                // Files (uploaded files related to issues)
                await SeedFilesAsync(context);

                // Feedback entries
                await SeedFeedbackAsync(context);

                // Additional demo events (15 future events)
                await SeedDemoEventsAsync(context);

                // Announcements
                await SeedDemoAnnouncementsAsync(context);

                Console.WriteLine("Demo data seeding completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding demo data: {ex.Message}");
            }
        }

        private static async Task SeedIssuesAsync(ApplicationDbContext context)
        {
            if (await context.Issues.AnyAsync())
            {
                Console.WriteLine("Issues already exist. Skipping issue seeding.");
                return;
            }

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

            context.Issues.AddRange(demoIssues);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {demoIssues.Count} issues.");
        }

        private static async Task SeedFilesAsync(ApplicationDbContext context)
        {
            if (await context.UploadedFiles.AnyAsync())
            {
                Console.WriteLine("Uploaded files already exist. Skipping file seeding.");
                return;
            }

            // Re-query the issues to ensure IDs are present
            var demoIssues = await context.Issues.OrderBy(i => i.CreatedAt).ToListAsync();
            if (!demoIssues.Any())
            {
                Console.WriteLine("No issues found to attach files to. Skipping file seeding.");
                return;
            }

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
                    IssueID = demoIssues.ElementAtOrDefault(1)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new UploadedFile
                {
                    FileName = "waterpipe.jpg",
                    MimeType = "image/jpeg",
                    Size = 980000,
                    FilePath = await CopySampleFileToUploads("waterpipe.jpg", "sample/images/waterpipe.jpg"),
                    IssueID = demoIssues.ElementAtOrDefault(2)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                new UploadedFile
                {
                    FileName = "dumping.jpg",
                    MimeType = "image/jpeg",
                    Size = 1150000,
                    FilePath = await CopySampleFileToUploads("dumping.jpg", "sample/images/dumping.jpg"),
                    IssueID = demoIssues.ElementAtOrDefault(3)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new UploadedFile
                {
                    FileName = "playground.jpg",
                    MimeType = "image/jpeg",
                    Size = 890000,
                    FilePath = await CopySampleFileToUploads("playground.jpg", "sample/images/playground.jpeg"),
                    IssueID = demoIssues.ElementAtOrDefault(4)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddHours(-6)
                }
            };

            context.UploadedFiles.AddRange(demoFiles);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {demoFiles.Count} uploaded files.");
        }

        private static async Task SeedFeedbackAsync(ApplicationDbContext context)
        {
            if (await context.Feedbacks.AnyAsync())
            {
                Console.WriteLine("Feedback entries already exist. Skipping feedback seeding.");
                return;
            }

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

            context.Feedbacks.AddRange(demoFeedback);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {demoFeedback.Count} feedback entries.");
        }

        private static async Task SeedDemoEventsAsync(ApplicationDbContext context)
        {
            if (await context.Events.AnyAsync())
            {
                Console.WriteLine("Events already exist. Skipping demo event seeding.");
                return;
            }

            Console.WriteLine("Seeding 15 demo events (future dates)...");


            // Exclude the categories Other, Youth, Arts
            var categories = new[] { "Community", "Culture", "Sports", "Education", "Environment", "Health", "Business", "Technology" };

            var demoEvents = new List<Event>();

            var baseDate = DateTime.Now.Date.AddDays(7); // start one week from now

            // Spread across multiple months with varied offsets
            var offsets = new int[] { 7, 14, 21, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180, 200 };

            // Realistic mock event data (titles, descriptions, locations)
            var mockData = new (string Title, string Description, string Location)[]
            {
                ("Clean-up Day", "Join neighbours for a community clean-up and recycling drive. Gloves and bags provided.", "Riverside Park"),
                ("Folk Music", "An evening of folk and cultural music performances by local bands and choirs.", "Town Hall Stage"),
                ("5K Run", "Family-friendly 5K run to promote active lifestyles. Registration opens 30 minutes before the race.", "Riverside Park Track"),
                ("Finance Talk", "Free seminar on budgeting and personal finance for adults.", "Hillcrest Community Centre"),
                ("Tree Planting", "Volunteer to help plant native trees to improve our local green spaces.", "Riverbank Grove"),
                ("Health Screen", "Free basic health checks, blood pressure and diabetes screening.", "Health Centre Lobby"),
                ("Biz Networking", "Meet local entrepreneurs and discover partnership opportunities.", "Conference Hall A"),
                ("IoT Meetup", "A talk and demo session on IoT applications for municipal services.", "Tech Hub, Innovation Floor"),
                ("BBQ Meet", "A casual community gathering to meet neighbours and local councillors.", "Southside Park"),
                ("Dance Showcase", "Performances by community cultural groups showcasing traditional and contemporary dance.", "Civic Theatre"),
                ("Basketball", "3-on-3 tournament for local teams. Age divisions for adults and teens.", "Sports Complex Court 1"),
                ("STEM Day", "Hands-on science and technology activities for students hosted by local schools.", "Science Centre"),
                ("River Workshop", "Learn about river conservation and how to get involved in local initiatives.", "Riverbank Centre"),
                ("Vax Clinic", "Walk-in vaccinations for flu and other routine immunisations.", "Municipal Health Clinic"),
                ("Business Expo", "Showcase of local businesses, services, and support organizations.", "Exhibition Hall")
            };

            for (int i = 0; i < 15; i++)
            {
                var cat = categories[i % categories.Length];
                var eventDate = DateOnly.FromDateTime(baseDate.AddDays(offsets[i]));
                var eventTime = TimeOnly.FromDateTime(baseDate.AddHours(9 + (i % 8))); // stagger times through the day

                var md = mockData[i];

                demoEvents.Add(new Event
                {
                    Title = md.Title,
                    Description = md.Description,
                    Category = cat,
                    Date = eventDate,
                    Time = eventTime,
                    Location = md.Location,
                    CreatedAt = DateTime.Now
                });
            }

            context.Events.AddRange(demoEvents);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {demoEvents.Count} demo events.");
        }

        private static async Task SeedDemoAnnouncementsAsync(ApplicationDbContext context)
        {
            try
            {
                // Check if announcements already exist
                if (await context.Announcements.AnyAsync())
                {
                    Console.WriteLine("Announcements already exist. Skipping announcement seeding.");
                    return;
                }

                Console.WriteLine("Seeding demo announcements...");

                var demoAnnouncements = new List<Announcement>
                {
                    new Announcement
                    {
                        Title = "Water Maintenance Notice",
                        Category = "Alert",
                        Content = "Scheduled water maintenance will occur on Sunday from 6 AM to 2 PM in the Hillcrest area.",
                        CreatedAt = DateTime.Now.AddDays(-2)
                    },
                    new Announcement
                    {
                        Title = "Community Meeting",
                        Category = "Information",
                        Content = "Monthly community meeting scheduled for next Friday at 7 PM in the Town Hall.",
                        CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    new Announcement
                    {
                        Title = "Road Closure Alert",
                        Category = "Alert",
                        Content = "Main Street will be closed for repairs from Monday to Wednesday. Use alternative routes.",
                        CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    new Announcement
                    {
                        Title = "New Recycling Program",
                        Category = "Information",
                        Content = "New recycling program starts next month. Collection days are Tuesdays and Fridays.",
                        CreatedAt = DateTime.Now.AddDays(-3)
                    },
                    new Announcement
                    {
                        Title = "Holiday Schedule",
                        Category = "Update",
                        Content = "Municipal offices will operate on reduced hours during the holiday period.",
                        CreatedAt = DateTime.Now.AddMinutes(-30)
                    }
                };

                // Add announcements to database
                context.Announcements.AddRange(demoAnnouncements);
                await context.SaveChangesAsync();

                Console.WriteLine($"Successfully seeded {demoAnnouncements.Count} demo announcements.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding demo announcements: {ex.Message}");
                throw;
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