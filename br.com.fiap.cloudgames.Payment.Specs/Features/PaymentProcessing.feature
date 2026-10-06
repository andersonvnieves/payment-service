Feature: Payment Processing

Scenario: Create a new payment with pending status
    Given an order "11111111-1111-1111-1111-111111111111" for user "22222222-2222-2222-2222-222222222222" with amount 100.00
    When the payment is created
    Then the payment status should be pending
    And the payment amount should be 100.00

Scenario: Successfully approve a pending payment
    Given a pending payment exists for order "11111111-1111-1111-1111-111111111111"
    When the payment is approved
    Then the payment status should be approved

Scenario: Successfully decline a pending payment
    Given a pending payment exists for order "22222222-2222-2222-2222-222222222222"
    When the payment is declined
    Then the payment status should be rejected

Scenario: Cannot approve an already processed payment
    Given an approved payment exists for order "33333333-3333-3333-3333-333333333333"
    When the payment is approved again
    Then a domain exception should be thrown with message "Payment already processed."
