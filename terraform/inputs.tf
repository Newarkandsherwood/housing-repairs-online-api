variable "storage_account_name" {
  type = string
}
variable "service_name" {
  type = string
}
variable "storage_container_name_production" {
  type = string
}
variable "storage_container_name_staging" {
  type = string
}
variable "resource_group_name" {
  type = string
}
variable "resource_group_location" {
  type = string
}
variable "gov_notify_key_production" {
  type = string
}
variable "gov_notify_key_staging" {
  type      = string
  sensitive = true
}
variable "addresses_api_url_production" {
  type = string
}
variable "addresses_api_url_staging" {
  type = string
}
variable "authentication_identifier_production" {
  type      = string
  sensitive = true
}
variable "authentication_identifier_staging" {
  type      = string
  sensitive = true
}
variable "internal_email_production" {
  type = string
}
variable "internal_email_staging" {
  type = string
}
variable "tenant_confirmation_email_notify_template_id" {
  type = string
}
variable "tenant_confirmation_sms_notify_template_id" {
  type = string
}
variable "tenant_internal_email_notify_template_id" {
  type = string
}
variable "communal_confirmation_email_notify_template_id" {
  type = string
}
variable "communal_confirmation_sms_notify_template_id" {
  type = string
}
variable "communal_internal_email_notify_template_id" {
  type = string
}
variable "leasehold_confirmation_email_notify_template_id" {
  type = string
}
variable "leasehold_confirmation_sms_notify_template_id" {
  type = string
}
variable "leasehold_internal_email_notify_template_id" {
  type = string
}
variable "days_until_image_expiry_production" {
  type = string
}
variable "days_until_image_expiry_staging" {
  type = string
}
variable "scheduling_api_url_production" {
  type = string
}
variable "scheduling_api_url_staging" {
  type = string
}
variable "jwt_secret_production" {
  type      = string
  sensitive = true
}
variable "jwt_secret_staging" {
  type      = string
  sensitive = true
}
variable "sentry_dsn" {
  type      = string
  sensitive = true
}
variable "sor_configuration_tenant_production" {
  type = string
}
variable "sor_configuration_tenant_staging" {
  type = string
}
variable "sor_configuration_communal_production" {
  type = string
}
variable "sor_configuration_communal_staging" {
  type = string
}
variable "allowed_appointment_slots_production" {
  type = string
}
variable "allowed_appointment_slots_staging" {
  type = string
}
variable "service_plan_name" {
  type = string
}
variable "azure_ad_tenant_id" {
  type      = string
  sensitive = true
}
