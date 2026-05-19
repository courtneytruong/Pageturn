# Pageturn

A full-stack blog application. Users can create, read, update, and delete blog posts.

## Stack

- **Frontend**: React, TypeScript, Vite, React Router, native fetch API
- **Backend**: ASP.NET Core Web API, C#, .NET 8
- **Storage**: JSON file (posts.json) — database to be added later

## File Structure

PageturnApi/
├── Controllers/PostsController.cs
├── Models/Post.cs, PostDtos.cs
├── Services/IPostService.cs, JsonPostService.cs
├── Data/posts.json
└── Program.cs

frontend/src/
├── components/PostCard.tsx, PostForm.tsx, ConfirmDelete.tsx
├── pages/Home.tsx, PostDetail.tsx, CreatePost.tsx, EditPost.tsx
├── services/api.ts
├── types/Post.ts
└── App.tsx

## Principles

- Follow SOLID principles throughout
- Single responsibility per class, component, and function
- Depend on abstractions — controllers depend on IPostService, not JsonPostService directly
- Keep components and functions small and focused
- Use DTOs for all API request bodies — never expose the model directly
- Shared PostForm component handles both create and edit

## Routing

- All post routes use ID: /posts/:id
- No slugs

## HTTP Client

- Use native fetch API only — no Axios or other HTTP libraries

## Deferred Features

- Database (EF Core + PostgreSQL) — JsonPostService will be swapped out, nothing else should change
- Book API integration (Google Books)
- Authentication
