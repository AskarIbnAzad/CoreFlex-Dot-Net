using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CoreFlex.Models;
using CoreFlex.IRepository;
using CoreFlex.Models.DatabaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoreFlex.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UsersController(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
    }

    // GET: Users
    public async Task<IActionResult> Index()
    {
        var users = await _userRepository.GetActiveAsync();
        return View(users);
    }

    // GET: Users/Create
    public async Task<IActionResult> Create()
    {
        await LoadRoles();
        return View();
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            // Check for duplicate username/email
            if (await _userRepository.IsUsernameUniqueAsync(user.Username))
            {
                if (await _userRepository.IsEmailUniqueAsync(user.Email))
                {
                    user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
                    user.CreatedBy = User.Identity?.Name ?? "System";
                    await _userRepository.AddAsync(user, user.CreatedBy);
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("Email", "Email already exists");
            }
            else
            {
                ModelState.AddModelError("Username", "Username already exists");
            }
        }

        await LoadRoles();
        return View(user);
    }

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        await LoadRoles();
        return View(user);
    }

    // POST: Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, User user)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Get existing user to preserve password if not changed
                var existingUser = await _userRepository.GetByIdAsync(id);

                if (!string.IsNullOrEmpty(user.PasswordHash)
                {
                    existingUser.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
                }

                // Update other properties
                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.RoleId = user.RoleId;
                existingUser.Status = user.Status;

                existingUser.UpdatedBy = User.Identity?.Name ?? "System";
                await _userRepository.UpdateAsync(existingUser, existingUser.UpdatedBy);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _userRepository.ExistsAsync(user.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        await LoadRoles();
        return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _userRepository.SoftDeleteAsync(id, User.Identity?.Name ?? "System");
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadRoles()
    {
        ViewBag.Roles = await _roleRepository.GetActiveAsync();
    }
}