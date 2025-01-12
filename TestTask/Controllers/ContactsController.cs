using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestTask.Context;
using TestTask.Models;
using TestTask.Repositories;

namespace TestTask.Controllers
{
    public class ContactsController : Controller
    {
        private readonly IContactRepository _contactRepository;

        public ContactsController(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            return View(await _contactRepository.GetAllAsync());
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contact contact)
        {
            if (ModelState.IsValid)
            {
                await _contactRepository.AddAsync(contact);
                await _contactRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _contactRepository.GetByIdAsync(id.Value);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Contact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _contactRepository.Update(contact);
                    await _contactRepository.SaveChangesAsync();
                    TempData["Success"] = "Contact updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Error"] = "An error occurred while updating the contact.";
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Invalid data. Please check your input.";
            return RedirectToAction(nameof(Index));
        }


        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("Error", "There is no contact with such id.");
            }

            var contact = await _contactRepository.GetByIdAsync(id.Value);
            if (contact == null)
            {
                return View("Error", "There is no contact with such id.");
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _contactRepository.DeleteAsync(id);
            await _contactRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile csvFile)
        {
            try
            {
                if (csvFile == null || csvFile.Length == 0)
                {
                    TempData["Error"] = "No file uploaded.";
                    return RedirectToAction(nameof(Index));
                }

                using (var reader = new StreamReader(csvFile.OpenReadStream()))
                {
                    List<Contact> contacts = new List<Contact>();

                    // Read and skip the header line
                    string headerLine = await reader.ReadLineAsync();
                    if (headerLine == null)
                    {
                        TempData["Error"] = "CSV file is empty.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Validate headers
                    string[] expectedHeaders = { "Name", "DateOfBirth", "Married", "Phone", "Salary" };
                    string[] actualHeaders = headerLine.Split(',');

                    if (!expectedHeaders.SequenceEqual(actualHeaders))
                    {
                        TempData["Error"] = "CSV file headers do not match the expected format.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Process data rows
                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(line)) continue;

                        var values = line.Split(',');
                        if (values.Length == expectedHeaders.Length)
                        {
                            contacts.Add(new Contact
                            {
                                Name = values[0],
                                DateOfBirth = DateTime.Parse(values[1]),
                                Married = bool.Parse(values[2]),
                                Phone = values[3],
                                Salary = decimal.Parse(values[4])
                            });
                        }
                    }

                    if (contacts.Any())
                    {
                        await _contactRepository.AddRangeAsync(contacts);
                        await _contactRepository.SaveChangesAsync();
                        TempData["Success"] = $"Successfully imported {contacts.Count} contacts.";
                    }
                    else
                    {
                        TempData["Error"] = "No valid contacts found in the CSV file.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while processing the CSV file. Details: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
