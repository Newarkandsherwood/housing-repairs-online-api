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
variable "gov_notify_key" {
  type = string
}
variable "addresses_api_url_production" {
  type = string
}
variable "addresses_api_url_staging" {
  type = string
}
variable "authentication_identifier" {
  type = string
}
variable "internal_email" {
  type = string
}
variable "confirmation_email_notify_template_id" {
  type = string
}
variable "confirmation_sms_notify_template_id" {
  type = string
}
variable "internal_email_notify_template_id" {
  type = string
}
variable "days_until_image_expiry" {
  type = string
}
variable "scheduling_api_url_production" {
  type = string
}
variable "scheduling_api_url_staginng" {
  type = string
}
variable "jwt_secret_production" {
  type = string
}
variable "jwt_secret_staging" {
  type = string
}
variable "sentry_dsn" {
  type = string
}
variable "sor_configuration" {
  type = string
}
