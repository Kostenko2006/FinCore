# FinCore Bank Testing Notes

Use this command to get screenshot-friendly test output with clear test names:

```bash
dotnet test FinCore.sln --logger "console;verbosity=detailed"
```

## 6.2 Тестування запуску системи та API

Automated tests:

- `6.2 Запуск системи: Swagger API документація доступна`
- `6.2 Запуск системи: головна адреса перенаправляє на Swagger`

What is checked:

- the API application starts in test environment;
- Swagger JSON is available;
- `/` redirects to `/swagger`.

## 6.3 Тестування автентифікації та ролей

Automated tests:

- `6.3 Автентифікація: демо-клієнт входить і отримує JWT`
- `6.3 Автентифікація: захищений API відхиляє запит без токена`
- `6.3 Ролі: клієнт не має доступу до адміністративного API`
- `6.3 Ролі: адміністратор має доступ до списку користувачів`

What is checked:

- demo client login;
- JWT token issue;
- protected endpoint rejects anonymous access;
- client role cannot open admin endpoints;
- admin role can open admin endpoints.

## 6.4 Тестування основних функцій системи

Automated tests:

- `6.4 Основні функції: клієнт бачить dashboard, рахунки, картки та операції`
- `6.4 Основні функції: клієнт створює новий рахунок`
- `6.4 Основні функції: клієнт випускає картку до свого рахунку`

What is checked:

- dashboard summary;
- accounts list;
- cards list;
- transactions list;
- account creation;
- card creation for user's own account.

## 6.5 Тестування переказів

Automated tests:

- `6.5 Перекази: внутрішній переказ змінює баланси і створює операції`
- `6.5 Перекази: система відхиляє переказ при недостатньому балансі`

What is checked:

- internal transfer between own accounts;
- source and target balances are updated;
- transfer creates transaction history records;
- insufficient balance is rejected.
