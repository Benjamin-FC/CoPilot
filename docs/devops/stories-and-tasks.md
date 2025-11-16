# DevOps Stories and Tasks

## Document Information
- **Version**: 1.0
- **Last Updated**: November 16, 2025
- **Project**: CRM API
- **Sprint Planning**: Q4 2025 - Q1 2026

## Table of Contents
- [Epic 1: CI/CD Pipeline](#epic-1-cicd-pipeline)
- [Epic 2: Infrastructure as Code](#epic-2-infrastructure-as-code)
- [Epic 3: Monitoring & Observability](#epic-3-monitoring--observability)
- [Epic 4: Security & Compliance](#epic-4-security--compliance)
- [Epic 5: Database Management](#epic-5-database-management)
- [Epic 6: Deployment Automation](#epic-6-deployment-automation)
- [Epic 7: Performance & Optimization](#epic-7-performance--optimization)

---

## Epic 1: CI/CD Pipeline

### Story 1.1: Implement GitHub Actions CI Pipeline

**As a** DevOps Engineer  
**I want to** automate the build and test process on every commit  
**So that** we can catch issues early and maintain code quality

**Acceptance Criteria**:
- Pipeline triggers on push to master and pull requests
- Builds solution for both Debug and Release configurations
- Runs all unit and integration tests
- Reports test results in GitHub Actions UI
- Fails the build if tests fail
- Caches NuGet packages for faster builds

**Tasks**:
- [ ] **Task 1.1.1**: Create `.github/workflows/ci.yml` workflow file
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: 
    - Set up workflow triggers (push, pull_request)
    - Configure .NET 8 SDK
    - Add restore, build, test steps
    - Configure test result reporting
  
- [ ] **Task 1.1.2**: Configure NuGet package caching
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: Use actions/cache to cache ~/.nuget/packages
  
- [ ] **Task 1.1.3**: Add build status badge to README
  - **Estimate**: 0.5 hours
  - **Priority**: Low
  - **Details**: Add GitHub Actions badge with build status

- [ ] **Task 1.1.4**: Configure branch protection rules
  - **Estimate**: 0.5 hours
  - **Priority**: High
  - **Details**: Require CI to pass before merge

**Definition of Done**:
- ✅ CI pipeline runs automatically on commits
- ✅ All tests execute and report results
- ✅ Build status visible in pull requests
- ✅ Master branch protected by CI checks

---

### Story 1.2: Implement Continuous Deployment to Azure

**As a** DevOps Engineer  
**I want to** automatically deploy successful builds to Azure  
**So that** new features reach production quickly and reliably

**Acceptance Criteria**:
- Deploy to staging environment on merge to master
- Deploy to production on tagged releases
- Use Azure App Service deployment
- Implement blue-green deployment strategy
- Rollback capability in case of failures

**Tasks**:
- [ ] **Task 1.2.1**: Create Azure deployment workflow
  - **Estimate**: 4 hours
  - **Priority**: High
  - **Details**:
    - Create `.github/workflows/deploy-staging.yml`
    - Configure Azure credentials as secrets
    - Add deployment steps for Azure App Service
  
- [ ] **Task 1.2.2**: Implement production deployment workflow
  - **Estimate**: 3 hours
  - **Priority**: High
  - **Details**:
    - Create `.github/workflows/deploy-production.yml`
    - Trigger on tags (v*.*.*)
    - Add manual approval step
  
- [ ] **Task 1.2.3**: Configure deployment slots for blue-green
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**:
    - Set up staging slot in Azure App Service
    - Configure slot swap in workflow
  
- [ ] **Task 1.2.4**: Add database migration step to deployment
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**:
    - Run EF Core migrations as part of deployment
    - Handle migration failures gracefully

- [ ] **Task 1.2.5**: Implement automated rollback on failure
  - **Estimate**: 3 hours
  - **Priority**: Medium
  - **Details**:
    - Health check after deployment
    - Auto-rollback if health check fails

**Definition of Done**:
- ✅ Staging deploys automatically on merge
- ✅ Production deploys on release tags
- ✅ Blue-green deployment working
- ✅ Rollback mechanism tested

---

### Story 1.3: Implement Code Quality Gates

**As a** Development Team Lead  
**I want to** enforce code quality standards in the pipeline  
**So that** we maintain high code quality and consistency

**Acceptance Criteria**:
- Run static code analysis (Roslyn analyzers)
- Check code coverage (minimum 70%)
- Run security vulnerability scanning
- Fail build if quality gates not met

**Tasks**:
- [ ] **Task 1.3.1**: Add .editorconfig with coding standards
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: Define C# formatting and naming rules
  
- [ ] **Task 1.3.2**: Enable Roslyn analyzers in CI
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Add analyzer packages, configure severity levels
  
- [ ] **Task 1.3.3**: Integrate code coverage reporting
  - **Estimate**: 3 hours
  - **Priority**: Medium
  - **Details**:
    - Use coverlet for coverage collection
    - Generate coverage report
    - Upload to Codecov or similar
  
- [ ] **Task 1.3.4**: Add security scanning with Snyk or Dependabot
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Configure vulnerability scanning for dependencies

**Definition of Done**:
- ✅ Code analysis runs on every build
- ✅ Coverage report generated and tracked
- ✅ Security vulnerabilities detected
- ✅ Quality metrics visible in PRs

---

## Epic 2: Infrastructure as Code

### Story 2.1: Create Azure Infrastructure with Terraform

**As a** DevOps Engineer  
**I want to** manage Azure infrastructure as code  
**So that** environments are reproducible and version-controlled

**Acceptance Criteria**:
- Terraform scripts provision all Azure resources
- Separate configurations for staging and production
- State stored in Azure Storage backend
- Documentation for running Terraform

**Tasks**:
- [ ] **Task 2.1.1**: Initialize Terraform project structure
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**:
    - Create `infrastructure/terraform/` directory
    - Set up modules for app service, database, networking
  
- [ ] **Task 2.1.2**: Create App Service configuration
  - **Estimate**: 3 hours
  - **Priority**: High
  - **Details**:
    - Define App Service plan
    - Configure App Service with deployment slots
    - Set app settings and connection strings
  
- [ ] **Task 2.1.3**: Create Azure SQL Database configuration
  - **Estimate**: 3 hours
  - **Priority**: High
  - **Details**:
    - Define SQL Server and database
    - Configure firewall rules
    - Set up geo-replication for production
  
- [ ] **Task 2.1.4**: Configure Azure Key Vault
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**:
    - Create Key Vault resource
    - Define secrets for API keys, connection strings
    - Set up access policies
  
- [ ] **Task 2.1.5**: Set up Terraform state backend
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Configure Azure Storage for remote state
  
- [ ] **Task 2.1.6**: Create environment-specific tfvars files
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: staging.tfvars, production.tfvars with environment configs

**Definition of Done**:
- ✅ All infrastructure provisioned via Terraform
- ✅ Staging and production environments defined
- ✅ State stored securely in Azure
- ✅ Documentation complete

---

### Story 2.2: Implement Docker Containerization

**As a** DevOps Engineer  
**I want to** containerize the CRM API  
**So that** deployments are consistent across environments

**Acceptance Criteria**:
- Dockerfile builds optimized API image
- Multi-stage build for smaller image size
- Image pushed to Azure Container Registry
- Docker Compose for local development

**Tasks**:
- [ ] **Task 2.2.1**: Create optimized Dockerfile
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**:
    - Multi-stage build (SDK → runtime)
    - Copy only necessary files
    - Set appropriate user (non-root)
  
- [ ] **Task 2.2.2**: Create .dockerignore file
  - **Estimate**: 0.5 hours
  - **Priority**: Low
  - **Details**: Exclude bin, obj, .vs, .git
  
- [ ] **Task 2.2.3**: Set up Azure Container Registry
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Create ACR via Terraform
  
- [ ] **Task 2.2.4**: Add Docker image build to CI pipeline
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**:
    - Build image on successful tests
    - Tag with commit SHA and version
    - Push to ACR
  
- [ ] **Task 2.2.5**: Create docker-compose.yml for development
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**:
    - Define API service
    - Add SQL Server service
    - Configure networking

**Definition of Done**:
- ✅ Docker image builds successfully
- ✅ Image size optimized (<200MB)
- ✅ Images stored in ACR
- ✅ Local dev with Docker Compose working

---

## Epic 3: Monitoring & Observability

### Story 3.1: Implement Application Insights Integration

**As a** DevOps Engineer  
**I want to** monitor application performance and errors  
**So that** we can quickly identify and resolve issues

**Acceptance Criteria**:
- Application Insights SDK integrated
- Custom metrics tracked (API requests, errors, Loops.so sync)
- Alerts configured for critical issues
- Dashboard created for key metrics

**Tasks**:
- [ ] **Task 3.1.1**: Add Application Insights NuGet package
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Install Microsoft.ApplicationInsights.AspNetCore
  
- [ ] **Task 3.1.2**: Configure Application Insights in Program.cs
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Add instrumentation key, enable telemetry
  
- [ ] **Task 3.1.3**: Add custom telemetry for Loops.so integration
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**:
    - Track sync success/failure
    - Track sync duration
    - Add custom properties (clientId, email)
  
- [ ] **Task 3.1.4**: Create availability tests
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Set up ping tests for /health endpoint
  
- [ ] **Task 3.1.5**: Configure alert rules
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**:
    - Error rate > 5%
    - Response time > 2s
    - Availability < 99%
  
- [ ] **Task 3.1.6**: Create Application Insights dashboard
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Key metrics, charts, request timeline

**Definition of Done**:
- ✅ Telemetry flowing to Application Insights
- ✅ Custom metrics tracked
- ✅ Alerts configured and tested
- ✅ Dashboard accessible to team

---

### Story 3.2: Implement Structured Logging with Serilog

**As a** Developer  
**I want to** have structured, searchable logs  
**So that** debugging production issues is easier

**Acceptance Criteria**:
- Serilog configured with multiple sinks
- Logs enriched with context (requestId, userId)
- Log levels configurable per environment
- Logs centralized in Azure Log Analytics

**Tasks**:
- [ ] **Task 3.2.1**: Add Serilog NuGet packages
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Serilog.AspNetCore, Azure sink, enrichers
  
- [ ] **Task 3.2.2**: Configure Serilog in Program.cs
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**:
    - Replace default logger
    - Configure sinks (Console, ApplicationInsights)
    - Set log levels
  
- [ ] **Task 3.2.3**: Add request enrichers
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: Add requestId, correlation ID, user context
  
- [ ] **Task 3.2.4**: Update existing log statements
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Convert to structured logging with properties
  
- [ ] **Task 3.2.5**: Configure log retention policies
  - **Estimate**: 1 hour
  - **Priority**: Low
  - **Details**: Set retention in Log Analytics (30 days dev, 90 days prod)

**Definition of Done**:
- ✅ Serilog configured and working
- ✅ Logs enriched with context
- ✅ Logs searchable in Azure portal
- ✅ Log retention configured

---

### Story 3.3: Implement Health Checks

**As a** DevOps Engineer  
**I want to** monitor application and dependency health  
**So that** load balancers can route traffic appropriately

**Acceptance Criteria**:
- /health endpoint returns status
- Checks database connectivity
- Checks external API availability (Loops.so)
- Integrated with Azure health probes

**Tasks**:
- [ ] **Task 3.3.1**: Add health check middleware
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Configure Microsoft.Extensions.Diagnostics.HealthChecks
  
- [ ] **Task 3.3.2**: Add database health check
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Check EF Core DbContext connectivity
  
- [ ] **Task 3.3.3**: Add Loops.so API health check
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Periodic ping to Loops.so API
  
- [ ] **Task 3.3.4**: Configure health check UI
  - **Estimate**: 2 hours
  - **Priority**: Low
  - **Details**: Add AspNetCore.HealthChecks.UI for visualization
  
- [ ] **Task 3.3.5**: Integrate with Azure App Service health probe
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Configure health probe settings in Azure

**Definition of Done**:
- ✅ /health endpoint returns JSON status
- ✅ All dependencies checked
- ✅ Azure health probe configured
- ✅ Unhealthy instances removed from load balancer

---

## Epic 4: Security & Compliance

### Story 4.1: Implement Secrets Management with Azure Key Vault

**As a** Security Engineer  
**I want to** store secrets in Azure Key Vault  
**So that** sensitive data is not exposed in configuration

**Acceptance Criteria**:
- All secrets moved to Key Vault
- Application retrieves secrets at runtime
- Key Vault access via Managed Identity
- Secrets rotation process documented

**Tasks**:
- [ ] **Task 4.1.1**: Create Azure Key Vault via Terraform
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Define Key Vault resource with access policies
  
- [ ] **Task 4.1.2**: Enable Managed Identity for App Service
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Configure system-assigned identity
  
- [ ] **Task 4.1.3**: Grant Key Vault access to Managed Identity
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Add access policy for app service identity
  
- [ ] **Task 4.1.4**: Store secrets in Key Vault
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**:
    - Loops API key
    - Database connection string
    - Application Insights key
  
- [ ] **Task 4.1.5**: Configure Key Vault provider in application
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Add Azure.Extensions.AspNetCore.Configuration.Secrets
  
- [ ] **Task 4.1.6**: Document secrets rotation process
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Create runbook for rotating secrets

**Definition of Done**:
- ✅ All secrets in Key Vault
- ✅ App retrieves secrets successfully
- ✅ No secrets in source code or appsettings
- ✅ Rotation process documented

---

### Story 4.2: Implement API Authentication with JWT

**As a** Security Engineer  
**I want to** secure API endpoints with JWT authentication  
**So that** only authorized clients can access the API

**Acceptance Criteria**:
- JWT Bearer authentication configured
- Token generation endpoint implemented
- Token validation on all endpoints
- Refresh token mechanism

**Tasks**:
- [ ] **Task 4.2.1**: Add JWT NuGet packages
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Microsoft.AspNetCore.Authentication.JwtBearer
  
- [ ] **Task 4.2.2**: Configure JWT authentication middleware
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**:
    - Set issuer, audience, signing key
    - Configure token validation parameters
  
- [ ] **Task 4.2.3**: Create authentication controller
  - **Estimate**: 3 hours
  - **Priority**: High
  - **Details**:
    - POST /api/auth/token (login)
    - POST /api/auth/refresh
  
- [ ] **Task 4.2.4**: Add [Authorize] attributes to controllers
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Protect all client endpoints
  
- [ ] **Task 4.2.5**: Implement refresh token storage and validation
  - **Estimate**: 3 hours
  - **Priority**: Medium
  - **Details**: Store refresh tokens in database
  
- [ ] **Task 4.2.6**: Update Swagger to support JWT
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: Add authorization button to Swagger UI

**Definition of Done**:
- ✅ JWT authentication working
- ✅ Unauthorized requests return 401
- ✅ Refresh tokens implemented
- ✅ Swagger supports auth

---

### Story 4.3: Implement Security Scanning

**As a** Security Engineer  
**I want to** automatically scan for security vulnerabilities  
**So that** we can address issues before they reach production

**Acceptance Criteria**:
- Dependency scanning in CI pipeline
- SAST (Static Application Security Testing) configured
- Container image scanning
- Vulnerability reports generated

**Tasks**:
- [ ] **Task 4.3.1**: Enable Dependabot on GitHub
  - **Estimate**: 0.5 hours
  - **Priority**: High
  - **Details**: Configure automatic dependency updates
  
- [ ] **Task 4.3.2**: Add Snyk security scanning to CI
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Scan NuGet packages for vulnerabilities
  
- [ ] **Task 4.3.3**: Implement SonarCloud for SAST
  - **Estimate**: 3 hours
  - **Priority**: Medium
  - **Details**: Integrate SonarCloud analysis in CI
  
- [ ] **Task 4.3.4**: Add Docker image scanning with Trivy
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Scan container images for vulnerabilities
  
- [ ] **Task 4.3.5**: Configure security reporting in GitHub
  - **Estimate**: 1 hour
  - **Priority**: Low
  - **Details**: Enable security tab, configure notifications

**Definition of Done**:
- ✅ Dependencies scanned automatically
- ✅ SAST reports generated
- ✅ Container images scanned
- ✅ Team notified of critical issues

---

## Epic 5: Database Management

### Story 5.1: Implement Automated Database Migrations

**As a** DevOps Engineer  
**I want to** automate database schema migrations  
**So that** deployments include database updates

**Acceptance Criteria**:
- Migrations run automatically during deployment
- Rollback capability for failed migrations
- Migration history tracked in database
- Zero-downtime migration strategy

**Tasks**:
- [ ] **Task 5.1.1**: Create EF Core migrations for initial schema
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Generate migration from current model
  
- [ ] **Task 5.1.2**: Add migration step to deployment pipeline
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Run dotnet ef database update in workflow
  
- [ ] **Task 5.1.3**: Implement migration validation
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Check pending migrations, validate schema
  
- [ ] **Task 5.1.4**: Create migration rollback script
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Script to revert last migration
  
- [ ] **Task 5.1.5**: Document migration best practices
  - **Estimate**: 2 hours
  - **Priority**: Low
  - **Details**: Guide for creating safe migrations

**Definition of Done**:
- ✅ Migrations run during deployment
- ✅ Migration history visible
- ✅ Rollback tested
- ✅ Documentation complete

---

### Story 5.2: Implement Database Backup Strategy

**As a** Database Administrator  
**I want to** automated database backups  
**So that** data can be recovered in case of failure

**Acceptance Criteria**:
- Automated daily full backups
- Transaction log backups every 15 minutes
- Backups stored in geo-redundant storage
- Restore process tested and documented

**Tasks**:
- [ ] **Task 5.2.1**: Configure Azure SQL automated backups
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Set retention period (30 days)
  
- [ ] **Task 5.2.2**: Enable geo-replication for production
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Configure secondary region
  
- [ ] **Task 5.2.3**: Create backup monitoring alerts
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: Alert on backup failures
  
- [ ] **Task 5.2.4**: Test database restore procedure
  - **Estimate**: 3 hours
  - **Priority**: High
  - **Details**: Perform point-in-time restore test
  
- [ ] **Task 5.2.5**: Document disaster recovery runbook
  - **Estimate**: 3 hours
  - **Priority**: High
  - **Details**: Step-by-step recovery procedures

**Definition of Done**:
- ✅ Backups running automatically
- ✅ Geo-replication configured
- ✅ Restore tested successfully
- ✅ DR runbook complete

---

### Story 5.3: Implement Database Performance Monitoring

**As a** Database Administrator  
**I want to** monitor database performance  
**So that** I can optimize queries and prevent issues

**Acceptance Criteria**:
- Query performance tracked
- Slow queries identified and logged
- Index usage monitored
- Alerts for performance degradation

**Tasks**:
- [ ] **Task 5.3.1**: Enable Query Store in Azure SQL
  - **Estimate**: 1 hour
  - **Priority**: High
  - **Details**: Configure query performance tracking
  
- [ ] **Task 5.3.2**: Set up database metrics in Azure Monitor
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Track DTU, connections, deadlocks
  
- [ ] **Task 5.3.3**: Create slow query alerts
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: Alert on queries > 5 seconds
  
- [ ] **Task 5.3.4**: Implement index recommendations monitoring
  - **Estimate**: 2 hours
  - **Priority**: Low
  - **Details**: Review and apply index suggestions
  
- [ ] **Task 5.3.5**: Create database performance dashboard
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Visualize key database metrics

**Definition of Done**:
- ✅ Query performance visible
- ✅ Slow queries identified
- ✅ Alerts configured
- ✅ Dashboard created

---

## Epic 6: Deployment Automation

### Story 6.1: Implement GitOps Deployment Strategy

**As a** DevOps Engineer  
**I want to** manage deployments through Git  
**So that** all changes are tracked and auditable

**Acceptance Criteria**:
- Deployment configuration in Git repository
- Changes require pull request review
- Automated validation of deployment configs
- Rollback via Git revert

**Tasks**:
- [ ] **Task 6.1.1**: Create deployment configuration structure
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Define YAML configs for environments
  
- [ ] **Task 6.1.2**: Implement configuration validation
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Schema validation in CI
  
- [ ] **Task 6.1.3**: Set up ArgoCD or Flux (optional)
  - **Estimate**: 4 hours
  - **Priority**: Low
  - **Details**: GitOps controller for K8s deployments
  
- [ ] **Task 6.1.4**: Configure deployment approval workflow
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Require approval for production deploys
  
- [ ] **Task 6.1.5**: Document GitOps workflow
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Guide for making deployment changes

**Definition of Done**:
- ✅ Deployment configs in Git
- ✅ Changes require approval
- ✅ Validation automated
- ✅ Workflow documented

---

### Story 6.2: Implement Feature Flags

**As a** Product Manager  
**I want to** control feature rollout with flags  
**So that** features can be enabled/disabled without deployment

**Acceptance Criteria**:
- Feature flag system integrated
- Flags configurable in Azure App Configuration
- Runtime flag evaluation
- A/B testing capability

**Tasks**:
- [ ] **Task 6.2.1**: Add Azure App Configuration NuGet package
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: Install Microsoft.Extensions.Configuration.AzureAppConfiguration
  
- [ ] **Task 6.2.2**: Configure App Configuration connection
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Set up connection, enable dynamic configuration
  
- [ ] **Task 6.2.3**: Create feature flag middleware
  - **Estimate**: 3 hours
  - **Priority**: Medium
  - **Details**: Check flags before executing actions
  
- [ ] **Task 6.2.4**: Add feature flags for Loops integration
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: Move Enabled flag to App Configuration
  
- [ ] **Task 6.2.5**: Create feature flag management UI
  - **Estimate**: 4 hours
  - **Priority**: Low
  - **Details**: Simple admin interface for toggling flags

**Definition of Done**:
- ✅ Feature flags working
- ✅ Flags configurable at runtime
- ✅ Loops integration controlled by flag
- ✅ Documentation complete

---

### Story 6.3: Implement Canary Deployments

**As a** DevOps Engineer  
**I want to** gradually roll out new versions  
**So that** issues can be detected with minimal impact

**Acceptance Criteria**:
- Traffic split between old and new versions
- Automated metrics comparison
- Automatic rollback on metric degradation
- Gradual traffic increase (10% → 50% → 100%)

**Tasks**:
- [ ] **Task 6.3.1**: Configure Azure Traffic Manager
  - **Estimate**: 3 hours
  - **Priority**: Medium
  - **Details**: Set up weighted routing
  
- [ ] **Task 6.3.2**: Create canary deployment workflow
  - **Estimate**: 4 hours
  - **Priority**: Medium
  - **Details**:
    - Deploy to canary slot
    - Route 10% traffic
    - Monitor metrics
  
- [ ] **Task 6.3.3**: Implement automated metric comparison
  - **Estimate**: 4 hours
  - **Priority**: Medium
  - **Details**: Compare error rates, latency between versions
  
- [ ] **Task 6.3.4**: Add automatic rollback logic
  - **Estimate**: 3 hours
  - **Priority**: High
  - **Details**: Rollback if error rate > threshold
  
- [ ] **Task 6.3.5**: Create canary deployment dashboard
  - **Estimate**: 2 hours
  - **Priority**: Low
  - **Details**: Visualize traffic split and metrics

**Definition of Done**:
- ✅ Canary deployments working
- ✅ Traffic gradually increased
- ✅ Auto-rollback functional
- ✅ Metrics comparison automated

---

## Epic 7: Performance & Optimization

### Story 7.1: Implement Response Caching

**As a** Performance Engineer  
**I want to** cache API responses  
**So that** repeated requests are served faster

**Acceptance Criteria**:
- Response caching middleware configured
- Cache headers set appropriately
- Redis cache for distributed caching
- Cache invalidation on updates

**Tasks**:
- [ ] **Task 7.1.1**: Add response caching middleware
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: Configure ResponseCaching middleware
  
- [ ] **Task 7.1.2**: Set cache profiles for endpoints
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Configure cache duration per endpoint
  
- [ ] **Task 7.1.3**: Set up Azure Redis Cache
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Provision Redis via Terraform
  
- [ ] **Task 7.1.4**: Configure distributed caching
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Use Redis for response cache
  
- [ ] **Task 7.1.5**: Implement cache invalidation
  - **Estimate**: 3 hours
  - **Priority**: Medium
  - **Details**: Clear cache on POST/PUT/DELETE

**Definition of Done**:
- ✅ Caching middleware configured
- ✅ Redis cache operational
- ✅ Cache invalidation working
- ✅ Performance improvement measured

---

### Story 7.2: Implement Rate Limiting

**As a** DevOps Engineer  
**I want to** limit API request rates  
**So that** the system is protected from abuse

**Acceptance Criteria**:
- Rate limiting per IP address
- Rate limiting per API key (when auth implemented)
- Configurable limits per environment
- 429 Too Many Requests response

**Tasks**:
- [ ] **Task 7.2.1**: Add rate limiting NuGet package
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: Install AspNetCoreRateLimit
  
- [ ] **Task 7.2.2**: Configure IP rate limiting
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Set limits per IP (100 req/min)
  
- [ ] **Task 7.2.3**: Configure API key rate limiting
  - **Estimate**: 2 hours
  - **Priority**: Low
  - **Details**: Different limits per client tier
  
- [ ] **Task 7.2.4**: Add rate limit headers to responses
  - **Estimate**: 1 hour
  - **Priority**: Low
  - **Details**: X-RateLimit-Limit, X-RateLimit-Remaining
  
- [ ] **Task 7.2.5**: Store rate limit data in Redis
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Use Redis for distributed rate limiting

**Definition of Done**:
- ✅ Rate limiting active
- ✅ 429 responses returned
- ✅ Rate limit headers present
- ✅ Distributed limiting working

---

### Story 7.3: Implement Database Connection Pooling Optimization

**As a** Performance Engineer  
**I want to** optimize database connection management  
**So that** database resources are used efficiently

**Acceptance Criteria**:
- Connection pool configured optimally
- Connection leaks prevented
- Connection metrics monitored
- Pool exhaustion alerts configured

**Tasks**:
- [ ] **Task 7.3.1**: Configure connection pool settings
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Set min/max pool size, timeout
  
- [ ] **Task 7.3.2**: Implement connection resilience
  - **Estimate**: 2 hours
  - **Priority**: High
  - **Details**: Add retry logic for transient failures
  
- [ ] **Task 7.3.3**: Add connection pool monitoring
  - **Estimate**: 2 hours
  - **Priority**: Medium
  - **Details**: Track active, idle connections
  
- [ ] **Task 7.3.4**: Configure pool exhaustion alerts
  - **Estimate**: 1 hour
  - **Priority**: Medium
  - **Details**: Alert when pool > 80% utilized
  
- [ ] **Task 7.3.5**: Load test connection pool under stress
  - **Estimate**: 3 hours
  - **Priority**: Medium
  - **Details**: Use k6 or JMeter for load testing

**Definition of Done**:
- ✅ Connection pool optimized
- ✅ Resilience policy configured
- ✅ Monitoring in place
- ✅ Load tested

---

## Sprint Planning Template

### Sprint N: [Dates]

**Sprint Goal**: [High-level objective]

**Selected Stories**:
1. Story X.X - [Priority] - [Story Points]
2. Story Y.Y - [Priority] - [Story Points]

**Total Story Points**: [Sum]

**Team Capacity**: [Available hours]

**Daily Standup Schedule**: [Time, platform]

**Sprint Review**: [Date, time]

**Sprint Retrospective**: [Date, time]

---

## Estimation Guide

### Story Points (Fibonacci Scale)
- **1 point**: Simple task, < 2 hours, minimal complexity
- **2 points**: Small task, 2-4 hours, low complexity
- **3 points**: Medium task, 4-8 hours, moderate complexity
- **5 points**: Large task, 1-2 days, significant complexity
- **8 points**: Very large task, 2-3 days, high complexity
- **13 points**: Epic-level, should be broken down

### Estimation Factors
- Technical complexity
- Unknown dependencies
- Testing requirements
- Documentation needs
- Team familiarity with technology

---

## Definition of Ready (DoR)

Stories are ready for sprint when:
- [ ] User story clearly written (As a... I want... So that...)
- [ ] Acceptance criteria defined
- [ ] Tasks broken down and estimated
- [ ] Dependencies identified
- [ ] Technical approach discussed
- [ ] Priority assigned

---

## Definition of Done (DoD)

Tasks are complete when:
- [ ] Code written and reviewed
- [ ] Unit tests written and passing
- [ ] Integration tests passing (if applicable)
- [ ] Documentation updated
- [ ] Deployed to staging environment
- [ ] Acceptance criteria verified
- [ ] Product owner approval obtained

---

## Risk Register

### High Priority Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Azure outage during deployment | High | Low | Multi-region deployment, rollback plan |
| Database migration failure | High | Medium | Test migrations on staging, rollback scripts |
| Key Vault access issues | High | Low | Fallback to app settings, test Managed Identity |
| Third-party API downtime (Loops) | Medium | Medium | Graceful degradation, queue-based retry |
| CI/CD pipeline failure | Medium | Medium | Manual deployment procedure documented |

---

## Tools & Technologies

### CI/CD
- **GitHub Actions**: Pipeline automation
- **Azure DevOps**: Alternative pipeline option

### Infrastructure
- **Terraform**: Infrastructure as Code
- **Azure CLI**: Resource management
- **Docker**: Containerization

### Monitoring
- **Application Insights**: APM and logging
- **Azure Monitor**: Metrics and alerts
- **Log Analytics**: Centralized logging
- **Serilog**: Structured logging

### Security
- **Azure Key Vault**: Secrets management
- **Snyk**: Dependency scanning
- **SonarCloud**: SAST
- **Trivy**: Container scanning

### Performance
- **Azure Redis Cache**: Distributed caching
- **k6 or JMeter**: Load testing
- **AspNetCoreRateLimit**: Rate limiting

---

## Contact & Support

**DevOps Team Lead**: [Name]  
**Email**: devops@example.com  
**Slack Channel**: #crm-api-devops  
**On-Call Rotation**: [PagerDuty/Schedule]

---

## Appendix

### Useful Commands

#### Terraform
```bash
# Initialize
terraform init

# Plan
terraform plan -var-file=staging.tfvars

# Apply
terraform apply -var-file=staging.tfvars

# Destroy
terraform destroy -var-file=staging.tfvars
```

#### Docker
```bash
# Build
docker build -t crm-api:latest .

# Run
docker run -p 8080:80 crm-api:latest

# Push to ACR
az acr login --name crmapiacr
docker push crmapiacr.azurecr.io/crm-api:latest
```

#### EF Core Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Rollback
dotnet ef database update PreviousMigration

# Generate SQL script
dotnet ef migrations script
```

#### Azure CLI
```bash
# Login
az login

# Deploy app
az webapp deployment source config-zip \
  --resource-group crm-api-rg \
  --name crm-api-prod \
  --src publish.zip

# View logs
az webapp log tail \
  --resource-group crm-api-rg \
  --name crm-api-prod
```

---

**Document Version**: 1.0  
**Last Review**: November 16, 2025  
**Next Review**: December 16, 2025
