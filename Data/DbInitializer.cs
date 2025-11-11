using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ST10296167_PROG7312_POE.Models;
using System.IO;

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
                    Status = IssueStatus.Resolved,
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new Issue
                {
                    Category = "Roads & Transport",
                    Description = "Streetlight has been out for over two weeks, creating unsafe conditions at night.",
                    Address = "456 Oak Avenue",
                    Suburb = "Westville",
                    Location = "456 Oak Avenue, Westville",
                    Status = IssueStatus.Resolved,
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new Issue
                {
                    Category = "Public Safety",
                    Description = "Burst water pipe flooding the street and causing water wastage.",
                    Address = "789 Pine Road",
                    Suburb = "Durban North",
                    Location = "789 Pine Road, Durban North",
                    Status = IssueStatus.InProgress,
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                new Issue
                {
                    Category = "Waste Management",
                    Description = "Illegal dumping site with household waste and construction debris.",
                    Address = "321 Cedar Street",
                    Suburb = "Pinetown",
                    Location = "321 Cedar Street, Pinetown",
                    Status = IssueStatus.InProgress,
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new Issue
                {
                    Category = "Parks & Recreation",
                    Description = "Playground equipment is damaged and unsafe for children to use.",
                    Address = "654 Maple Drive",
                    Suburb = "Kloof",
                    Location = "654 Maple Drive, Kloof",
                    Status = IssueStatus.Submitted,
                    CreatedAt = DateTime.Now.AddHours(-6)
                },
                // CLUSTER 1: Road Damage in Westville (3 issues within 3 days)
                new Issue
                {
                    Category = "Road Damage",
                    Description = "Deep pothole on the main intersection causing tire damage and traffic slowdowns.",
                    Address = "12 Sunset Boulevard",
                    Suburb = "Westville",
                    Location = "12 Sunset Boulevard, Westville",
                    Status = IssueStatus.Submitted,
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
                new Issue
                {
                    Category = "Road Damage",
                    Description = "Multiple cracks and surface deterioration making the road unsafe for cyclists.",
                    Address = "45 Valley Road",
                    Suburb = "Westville",
                    Location = "45 Valley Road, Westville",
                    Status = IssueStatus.Submitted,
                    CreatedAt = DateTime.Now.AddDays(-29)
                },
                new Issue
                {
                    Category = "Road Damage",
                    Description = "Road surface collapsed after recent rainfall, creating a large sinkhole.",
                    Address = "78 Ridge Avenue",
                    Suburb = "Westville",
                    Location = "78 Ridge Avenue, Westville",
                    Status = IssueStatus.InProgress,
                    CreatedAt = DateTime.Now.AddDays(-28)
                },

                // CLUSTER 2: Water & Sanitation in Durban North (2 issues within 3 days)
                new Issue
                {
                    Category = "Water & Sanitation",
                    Description = "Sewage leak in residential area causing foul odors and health concerns.",
                    Address = "234 Beach Road",
                    Suburb = "Durban North",
                    Location = "234 Beach Road, Durban North",
                    Status = IssueStatus.InProgress,
                    CreatedAt = DateTime.Now.AddDays(-25)
                },
                new Issue
                {
                    Category = "Water & Sanitation",
                    Description = "Water pressure extremely low in the area, affecting multiple households.",
                    Address = "567 Ocean Drive",
                    Suburb = "Durban North",
                    Location = "567 Ocean Drive, Durban North",
                    Status = IssueStatus.Submitted,
                    CreatedAt = DateTime.Now.AddDays(-24)
                },

                // CLUSTER 3: Waste Management in Pinetown (2 issues within 3 days)
                new Issue
                {
                    Category = "Waste Management",
                    Description = "Garbage bins not collected for two weeks, causing overflow and attracting pests.",
                    Address = "89 Station Road",
                    Suburb = "Pinetown",
                    Location = "89 Station Road, Pinetown",
                    Status = IssueStatus.Submitted,
                    CreatedAt = DateTime.Now.AddDays(-20)
                },
                new Issue
                {
                    Category = "Waste Management",
                    Description = "Broken recycling bins need replacement, recyclables being scattered by wind.",
                    Address = "156 Market Street",
                    Suburb = "Pinetown",
                    Location = "156 Market Street, Pinetown",
                    Status = IssueStatus.Submitted,
                    CreatedAt = DateTime.Now.AddDays(-18)
                },

                // CLUSTER 4: Public Safety in various suburbs (2 issues within 3 days)
                new Issue
                {
                    Category = "Public Safety",
                    Description = "Broken fence at community park allowing unauthorized access and vandalism.",
                    Address = "23 Park Lane",
                    Suburb = "Hillcrest",
                    Location = "23 Park Lane, Hillcrest",
                    Status = IssueStatus.Resolved,
                    CreatedAt = DateTime.Now.AddDays(-15)
                },
                new Issue
                {
                    Category = "Public Safety",
                    Description = "Inadequate lighting on pedestrian pathway creating safety concerns at night.",
                    Address = "67 Garden Road",
                    Suburb = "Kloof",
                    Location = "67 Garden Road, Kloof",
                    Status = IssueStatus.Submitted,
                    CreatedAt = DateTime.Now.AddDays(-13)
                },

                // STANDALONE ISSUES (spread across time, different categories)
                new Issue
                {
                    Category = "Electricity",
                    Description = "Frequent power outages in the neighborhood lasting several hours each time.",
                    Address = "345 Power Line Road",
                    Suburb = "Hillcrest",
                    Location = "345 Power Line Road, Hillcrest",
                    Status = IssueStatus.InProgress,
                    CreatedAt = DateTime.Now.AddDays(-10)
                },
                new Issue
                {
                    Category = "Public Transport",
                    Description = "Bus stop shelter is damaged and provides no protection from weather.",
                    Address = "12 Transit Avenue",
                    Suburb = "Pinetown",
                    Location = "12 Transit Avenue, Pinetown",
                    Status = IssueStatus.Submitted,
                    CreatedAt = DateTime.Now.AddDays(-7)
                },
                new Issue
                {
                    Category = "Environment",
                    Description = "Trees overhanging power lines pose risk of outages and fire hazard.",
                    Address = "89 Forest Road",
                    Suburb = "Kloof",
                    Location = "89 Forest Road, Kloof",
                    Status = IssueStatus.Submitted,
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new Issue
                {
                    Category = "Utilities",
                    Description = "Drainage system blocked causing flooding during rainfall.",
                    Address = "234 River Street",
                    Suburb = "Durban North",
                    Location = "234 River Street, Durban North",
                    Status = IssueStatus.Submitted,
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new Issue
                {
                    Category = "Other",
                    Description = "Graffiti vandalism on municipal building walls requires cleaning and repainting.",
                    Address = "567 Town Square",
                    Suburb = "Westville",
                    Location = "567 Town Square, Westville",
                    Status = IssueStatus.Submitted,
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new Issue
                {
                    Category = "Roads & Transport",
                    Description = "Traffic light malfunctioning at busy intersection causing traffic congestion and near-accidents.",
                    Address = "90 Highway Junction",
                    Suburb = "Pinetown",
                    Location = "90 Highway Junction, Pinetown",
                    Status = IssueStatus.InProgress,
                    CreatedAt = DateTime.Now.AddHours(-12)
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

            ClearUploadsFolder();

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
                },
                // File for Issue 5 (Road Damage Cluster - Westville)
                new UploadedFile
                {
                    FileName = "pothole1.jpg",
                    MimeType = "image/jpeg",
                    Size = 1024000,
                    FilePath = await CopySampleFileToUploads("pothole1.jpg", "sample/images/pothole1.jpg"),
                    IssueID = demoIssues.ElementAtOrDefault(5)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
        
                // File for Issue 6 (Road Damage Cluster - Westville)
                new UploadedFile
                {
                    FileName = "pothole2.jpg",
                    MimeType = "image/jpeg",
                    Size = 856000,
                    FilePath = await CopySampleFileToUploads("pothole2.jpg", "sample/images/pothole2.jpg"),
                    IssueID = demoIssues.ElementAtOrDefault(6)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddDays(-29)
                },
        
                // File for Issue 8 (Water & Sanitation Cluster - Durban North)
                new UploadedFile
                {
                    FileName = "waterpipe.jpg",
                    MimeType = "image/jpeg",
                    Size = 980000,
                    FilePath = await CopySampleFileToUploads("waterpipe.jpg", "sample/images/waterpipe.jpg"),
                    IssueID = demoIssues.ElementAtOrDefault(8)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddDays(-25)
                },
        
                // File for Issue 10 (Waste Management Cluster - Pinetown)
                new UploadedFile
                {
                    FileName = "dumping.jpg",
                    MimeType = "image/jpeg",
                    Size = 1150000,
                    FilePath = await CopySampleFileToUploads("dumping.jpg", "sample/images/dumping.jpg"),
                    IssueID = demoIssues.ElementAtOrDefault(10)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddDays(-20)
                },
        
                // File for Issue 12 (Public Safety)
                new UploadedFile
                {
                    FileName = "playground.jpg",
                    MimeType = "image/jpeg",
                    Size = 890000,
                    FilePath = await CopySampleFileToUploads("playground.jpg", "sample/images/playground.jpeg"),
                    IssueID = demoIssues.ElementAtOrDefault(12)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddDays(-15)
                },
        
                // File for Issue 13 (Public Safety)
                new UploadedFile
                {
                    FileName = "streetlight.jpg",
                    MimeType = "image/jpeg",
                    Size = 1200000,
                    FilePath = await CopySampleFileToUploads("streetlight.jpg", "sample/images/streetlight.jpeg"),
                    IssueID = demoIssues.ElementAtOrDefault(13)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddDays(-13)
                },
        
                // Files for Issue 19 (Roads & Transport)
                new UploadedFile
                {
                    FileName = "streetlight.jpg",
                    MimeType = "image/jpeg",
                    Size = 1200000,
                    FilePath = await CopySampleFileToUploads("streetlight.jpg", "sample/images/streetlight.jpeg"),
                    IssueID = demoIssues.ElementAtOrDefault(19)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddHours(-12)
                },
                new UploadedFile
                {
                    FileName = "pothole1.jpg",
                    MimeType = "image/jpeg",
                    Size = 1024000,
                    FilePath = await CopySampleFileToUploads("pothole1.jpg", "sample/images/pothole1.jpg"),
                    IssueID = demoIssues.ElementAtOrDefault(19)?.ID ?? demoIssues[0].ID,
                    CreatedAt = DateTime.Now.AddHours(-12)
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

        private static void ClearUploadsFolder() 
        {
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string uploadsFolder = Path.Combine(currentDirectory, "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    return;
                }

                foreach (var file in Directory.GetFiles(uploadsFolder))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                    }
                }

                foreach (var directory in Directory.GetDirectories(uploadsFolder))
                {
                    try
                    {
                        Directory.Delete(directory, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting directory {directory}: {ex.Message}");
                    }
                }

                Console.WriteLine("Cleared uploads folder before seeding files.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing uploads folder: {ex.Message}");
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