# GitHub Provider Docs
# https://registry.terraform.io/providers/integrations/github/latest

terraform {
required_providers {
    github = {
    source  = "integrations/github"
    version = "6.9.0"
    }
}
}
variable "github_token" {
    sensitive = true
}

provider "github" {
    token = var.github_token
}

resource "github_repository" "remote" {
    name       = "YOKO-markert"
    visibility = "public"
}

output "remote_repo" {
    value = github_repository.remote.http_clone_url
}

locals {
    workers = [
    "IvanKalytaYOKO",
    "jespergran98",
    ]
}

resource "github_repository_collaborator" "admins" {
    # for_each is HashiCorp Configuration Language's (HCL) "hacky" variation of a loop
    for_each = toset(local.workers)

    repository = github_repository.remote.name
    username   = each.value
    permission = "admin"
    }