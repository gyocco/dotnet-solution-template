# Git conventions

Practical guide for branches, commit messages, pull requests, and releases for this repository.

## Branching strategy

We use a lightweight GitHub Flow with permanent branches and short-lived branches.

- Permanent branches:

  - `main` — production branch, protected
  - `dev` — integration branch for ongoing work (merge feature/fix here), protected
  - `test` — stabilization branch used for system/integration testing, protected
  - `qa` — pre‑release validation/UAT branch, protected
  - `sprint/<sprint-id>` — optional time‑boxed integration branch for a sprint; protected while active

- Short‑lived branches (delete after merge):
  - `feature/<pbi-id>-<kebab-summary>`
  - `fix/<pbi-id>-<kebab-summary>`
  - `docs/<kebab-summary>`
  - `refactor/<kebab-summary>`
  - `chore/<kebab-summary>`
  - `test/<kebab-summary>`
  - `hotfix/<version>-<kebab-summary>` (urgent patch off `main`)
  - `release/<version>` (optional, for coordinated releases)

Naming rules:

- kebab-case, no spaces, ASCII only
- If tracked by a PBI (Product Backlog Item), prefix the ID: `feature/pbi-123-add-products-search`
- Keep names under ~50 chars preferred
- Sprint branch naming suggestions: `sprint/2025-08-01`
- Note: PBI = Product Backlog Item

Examples:

- `feature/pbi-123-add-products-search`
- `fix/pbi-456-handle-edge-case`
- `chore/ci-update-dotnet-sdk`
- `docs/readme-badges`
- `refactor/services-split-products-services`
- `test/webapi-integration-products-endpoints`
- `sprint/2025-08-01`
- `release/1.3.0`
- `hotfix/1.3.1-null-ref`

## Flow

1. Start from latest dev:
   - `git checkout dev`
   - `git pull origin dev`
2. Create a new branch:
   - `git switch -c feature/pbi-123-add-products-search`
3. Commit early and often using Conventional Commits (see below).
4. Push and open a PR to a `sprint/<sprint-id>` branch or `dev` (depending on project context). Open a PR to `main` only for hotfixes or release PRs.
5. Ensure the PR passes checks and reviews, then “Squash and merge”.
6. Delete the branch after merge.

Environment promotions (typical):

- If using sprint branches: `feature/*` and `fix/*` → `sprint/<sprint-id>` → `dev` → `test` → `qa` → `main` (tag as needed)
- If not using sprint branches: `feature/*` and `fix/*` → `dev` → `test` → `qa` → `main`
- Hotfixes may branch from `main`, then back‑merge to `dev`/`test`/`qa`.

### Sprint branches (optional)

- Create from `dev` at sprint start: `git switch -c sprint/2025-08 && git push -u origin sprint/2025-08`
- Merge PBI branches into the sprint branch via PR throughout the sprint
- Periodically sync from `dev` to keep up to date (or rebase sprint on `dev` if preferred)
- End of sprint: merge `sprint/<sprint-id>` back into `dev` via PR (squash recommended)
- Optionally tag a pre‑release off the sprint branch if needed
- Close the sprint by deleting or archiving the `sprint/<sprint-id>` branch after merge

Hotfixes:

- Branch from `main`: `hotfix/1.3.1-null-ref`
- Open PR to `main`; after merge, tag a patch release.
- Backport to `dev`/`test`/`qa` as applicable.

Optional release branch (when coordinating a cut):

- Create `release/1.4.0` from `main`
- Only allow fixes, docs, version bumps on `release/*`
- Tag `v1.4.0` from release branch; then merge back to `main` (no fast-forward)

## Commit messages (Conventional Commits)

Format:

```
type(scope): short summary
<blank line>
Longer body (optional, wrap ~72)
<blank line>
Footer(s): include PBI references e.g, Task #1234
```

Rules:

- type is one of: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`
- scope is optional; use an area or project portion (e.g., `webapi`, `services`, `infra`, `ci`)
- Subject in imperative mood, <= 72 chars, no trailing period
- Reference issues in footers: `Closes #123`, `Relates-to #456`

Examples:

- `feat(webapi): add GET /api/demos/{id}`
- `fix(services): guard null demo result to prevent NRE`
- `chore: add .gitignore and repo boilerplate`
- `refactor(data): split repository responsibilities`
- `docs: add local run and debugging instructions`
- `feat(webapi): remove legacy v1 endpoints`

Useful footers:

- `TASK #123`, `BUG #123`, `US #123`, `PBI #123`
- `BREAKING CHANGE: <details>`
- `IMPORTANT NOTE: <details>`
- `Co-authored-by: Name`

## Pull requests

- Title: mirror a Conventional Commit for the squash-merge commit
  - Example: `feat(webapi): add demo search endpoint`
- Description should include:
  - What and why (link to issue)
  - How (brief approach)
  - Risks/rollback plan
  - Testing notes (screenshots/logs if applicable)
- Link issues with keywords (`Closes #123`) so they auto-close on merge.
- Keep PRs small and focused (< ~400 LOC if possible).
- Checks:
  - Build and unit tests must pass
  - Lint/format (if configured) must pass
- Reviews:
  - At least one reviewer for non-trivial changes
  - Security-sensitive changes require explicit approval

Suggested PR template (you can add this under `.github/PULL_REQUEST_TEMPLATE.md`):

```md
## Summary

What changed and why?

## Risks/Rollback

- Risks:
- Rollback plan:

Closes #123
```

## Releases and versioning

- Semantic Versioning: MAJOR.MINOR.PATCH
- Conventional Commits drive version bumps (if using release automation):
  - `feat` -> minor
  - `fix` -> patch
  - `BREAKING CHANGE` or `!` -> major
- Tags: `v1.3.0`, `v1.3.1`
- Release notes: generated from commit history/PR titles

Manual tagging example:

- `git tag v1.3.0`
- `git push origin v1.3.0`

## Dotnet specifics

- Global scope suggestions: `webapi`, `services`, `data`, `infra`, `ci`, `docs`
- Feature scope suggestions: `products-list`, `products-detail`, `orders`, `payments`
- Commit examples:
  - `build: bump TargetFramework to net8.0`
  - `test(webapi): add integration tests for DemosController`
  - `ci: add GitHub Actions to build and test`

## Branch protection

- Apply protection rules to `main`, `dev`, `test`, `qa`, and (optionally) active `sprint/*` branches (no direct pushes)
- Require status checks to pass (build, tests)
- Require at least 1 approval
- Require linear history, use squash merge (optional)
- Dismiss stale reviews on new commits
- Include admins (optional)

## Practical command snippets

Create and push a feature branch:

```sh
git checkout dev
git pull
git switch -c feature/pbi-123-add-products-search
# work & commit
git push -u origin feature/pbi-123-add-products-search
```

Create and push a sprint branch from dev:

```sh
git checkout dev
git pull
git switch -c sprint/2025-08
git push -u origin sprint/2025-08
```

Conventional Commit:

```sh
git commit -m "feat(webapi): add demo search endpoint"
```

Open PR:

- Push branch, open PR to `sprint/<sprint-id>` or `dev` per project context; ensure title follows Conventional Commits.

Squash merge:

- Use the PR title as the final commit message.

Tag a release:

```sh
git tag v1.3.0
git push origin v1.3.0
```

## FAQ

- Q: When do I use `chore` vs `fix`?
  - `chore` is for repo maintenance that doesn’t affect runtime behavior (configs, CI, docs tooling).
  - `fix` is for bug fixes that change behavior.
- Q: When do I use `chore` vs `refactor`?
  - `chore` is for maintenance tasks that don't change code quality.
  - `refactor` is for code changes that improve structure and performance without altering behavior.
- Q: Which branch should I use as parent of my new feature branch?
  - Use `dev` or `sprint/*` for new features (depending on the project context) and `main` for urgent hot fixes.
- Q: Where do I put breaking change details?
  - In the body with a `BREAKING CHANGE:` footer, or use `!` in the header plus details in the body.
