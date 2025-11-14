# CRM API Documentation

This folder contains all documentation for the CRM API project.

## 📁 Documentation Structure

```
docs/
├── README.md                 # This file - documentation overview
├── architecture/             # Architecture diagrams and design
│   ├── system-architecture.md
│   ├── data-model.md
│   └── component-diagram.md
├── tech-stack/               # Technology stack details
│   ├── overview.md
│   ├── frontend.md
│   └── backend.md
├── plan/                     # Project planning documents
│   ├── detailed-plan.md
│   ├── milestones.md
│   └── acceptance-criteria.md
├── tasks/                    # Task management and tracking
│   ├── task-list.md
│   └── progress-tracking.md
├── api/                      # API documentation
│   ├── endpoints.md
│   ├── contracts.md
│   └── examples.md
├── development/              # Development guides
│   ├── setup.md
│   ├── coding-standards.md
│   └── testing.md
└── deployment/               # Deployment and operations
    ├── docker.md
    ├── ci-cd.md
    └── production.md
```

## 🚀 Quick Navigation

- [System Architecture](./architecture/system-architecture.md)
- [Technology Stack](./tech-stack/overview.md)
- [Detailed Project Plan](./plan/detailed-plan.md)
- [Current Tasks](./tasks/task-list.md)
- [API Documentation](./api/endpoints.md)
- [Development Setup](./development/setup.md)

## 📋 Project Overview

**CRM API** is a modern web application for managing contact information with the following key characteristics:

-- **Current implementation**: ASP.NET Core Web API (CRM backend)
-- **Planned frontend**: Next.js 14+ with TypeScript and Material UI (not yet implemented in this repository)
-- **Database**: In-memory for development, easily migratable to SQL databases
-- **Features**: Contact search, CRUD operations, responsive design, SEO-friendly

## 🎯 Core Features

1. **Contact Management**: Search, view, create, update, and delete contact records
2. **Advanced Search**: Full-text search across multiple contact fields
3. **Responsive UI**: Material UI components with mobile-friendly design
4. **Server-Side Rendering**: Fast initial loads and SEO optimization
5. **Type Safety**: Full TypeScript coverage from frontend to backend

## 📊 Current Status

- ✅ Architecture planning completed
- ✅ Technology stack selected
- ✅ Detailed implementation plan created
- ✅ Backend CRM API development completed
- 🔄 Frontend development pending

### Frontend Roadmap (Planned)

The detailed plan for the future frontend lives in the root-level document `PEO_Clients_Plan.md`.

Planned frontend highlights:

- Next.js 14+ with App Router and TypeScript
- Material UI (MUI) v5 for the UI layer
- Client pages such as `/clients`, `/clients/new`, and `/clients/[id]`
- Features: search, sort, paginate, create, view, and edit clients against the existing CRM API

Note: this frontend is **planned only** and is **not yet implemented in this repository**.

## 🤝 Contributing

All project documentation follows these principles:
- Clear, concise, and actionable
- Version-controlled with the codebase
- Regularly updated as the project evolves
- Accessible to both technical and non-technical stakeholders

## 📞 Support

For questions about the documentation or project, refer to the specific documentation sections or contact the development team.
