# תכנון מיקרו-סרביסים – Chinese Sale

## 1. מצב נוכחי (Monolith)

היום כל האפליקציה רצה כ-**Monolith** יחיד:

```
Angular SPA  →  SaleApi (ASP.NET Core 8)  →  SQL Server (SaleDB)
                    │
                    ├── Controllers (8)
                    ├── Services (8)
                    ├── Repositories (7)
                    └── SaleContextDB (7 טבלאות)
```

**טבלאות:** Users, Gifts, Orders, Bags, Categories, Doners, Winners

**תלויות cross-domain קיימות:**
- `BagService` → `OrderRepository` + `RandomRepository` (checkout)
- `RandomService` → `UserRepository`, `GiftRepository`, `BagRepository`, `EmailService`
- `Gift` → FK ל-`Doner`, `Category`
- `Order` → FK ל-`User`, `Gift`

---

## 2. עקרונות לחלוקה

| עקרון | יישום |
|-------|--------|
| **Bounded Context** | כל שירות = תחום עסקי אחד |
| **Database per Service** | לכל מיקרו-סרביס DB נפרד (SQL או MongoDB) |
| **API Gateway** | נקודת כניסה אחת ל-Angular |
| **Event-Driven** | פעולות cross-service דרך events (RabbitMQ / Azure Service Bus) |
| **Saga Pattern** | ל-checkout ולהגרלה – transactions מבוזרות |

---

## 3. מיקרו-סרביסים מוצעים

### 3.1 User Service (שירות משתמשים)

**אחריות:** הרשמה, התחברות, JWT, ניהול פרופיל, תפקידים (Admin/User)

| פריט | פירוט |
|------|--------|
| **Entities** | User |
| **DB** | `users_db` – collection/table Users |
| **API** | `POST /auth/login`, `POST /auth/register`, CRUD `/users` |
| **Controllers נוכחיים** | `AuthController`, `UserController` |
| **תלויות יוצאות** | אין (שירות עצמאי) |
| **Events שמפרסם** | `UserRegistered`, `UserDeleted` |

**הערות:** TokenService נשאר כאן. שאר השירותים מאמתים JWT דרך Gateway.

---

### 3.2 Catalog Service (שירות קטalog – מתנות, קטגוריות, תורמים)

**אחריות:** ניהול מתנות, קטגוריות, תורמים, תמונות

| פריט | פירוט |
|------|--------|
| **Entities** | Gift, Category, Doner |
| **DB** | `catalog_db` |
| **API** | `/gifts`, `/categories`, `/doners` + חיפושים |
| **Controllers נוכחיים** | `GiftController`, `CategoryController`, `DonerController` |
| **Storage** | Blob storage לתמונות (S3 / Azure Blob) במקום `wwwroot` |
| **Events שמפרסם** | `GiftCreated`, `GiftUpdated`, `GiftDeleted` |
| **Events שמאזין** | `GiftDrawn` → סימון מתנה כ"הוגרלה" / הסתרה מהקטalog |

**למה לאחד?** Gift, Category ו-Doner שייכים לאותו bounded context של "קטalog מוצרים". ב-monolith הם כבר קשורים ב-FK.

---

### 3.3 Cart Service (שירות עגלת קניות)

**אחריות:** הוספה/הסרה/עדכון כמות בעגלה, שליפת סל לפי משתמש

| פריט | פירוט |
|------|--------|
| **Entities** | Bag (CartItem) |
| **DB** | `cart_db` – Redis אופציונלי לביצועים |
| **API** | `GET /cart/{userId}`, `POST /cart/add`, `DELETE /cart/{id}` |
| **Controllers נוכחיים** | `BagController` (ללא checkout) |
| **תלויות** | קריאת HTTP ל-Catalog: `GET /gifts/{id}` (ולידציה + הצגה) |
| **Events שמפרסם** | `CheckoutRequested` |
| **Events שמאזין** | `GiftDrawn` → הסרת מתנה מכל העגלות |

**שינוי מ-monolith:** Checkout **לא** יישאר כאן – יעבור ל-Order Service דרך Saga.

---

### 3.4 Order Service (שירות הזמנות)

**אחריות:** יצירת הזמנות (כרטיסי הגרלה), היסטוריה, דוחות פופולריות/מחיר

| פריט | פירוט |
|------|--------|
| **Entities** | Order |
| **DB** | `orders_db` |
| **API** | `GET /orders`, `/orders/history/{userId}`, `/orders/by-gift/{giftId}`, sort endpoints |
| **Controllers נוכחיים** | `OrderController` + לוגיקת checkout מ-`BagService` |
| **Events שמאזין** | `CheckoutRequested` → Saga |
| **Events שמפרסם** | `OrderCreated`, `OrdersPlaced` (קבוצת הזמנות) |
| **תלויות** | User Service (validate userId), Catalog Service (validate giftId) |

**Checkout Saga:**
```
1. Cart Service  →  CheckoutRequested { userId, items[] }
2. Order Service →  validate user + gifts
3. Order Service →  create N Order rows per quantity + OrderGroupId
4. Order Service →  OrdersPlaced
5. Cart Service  →  clear cart (on success)
6. on failure    →  compensating: rollback / retry
```

---

### 3.5 Raffle Service (שירות הגרלות)

**אחריות:** הגרלה, רשימת זוכים, בדיקה אם מתנה הוגרלה

| פריט | פירוט |
|------|--------|
| **Entities** | Winner |
| **DB** | `raffle_db` |
| **API** | `POST /raffle/{giftId}`, `GET /raffle/is-drawn/{giftId}`, `GET /raffle/winners` |
| **Controllers נוכחיים** | `RandomController` |
| **תלויות** | Order Service: `GET /orders/by-gift/{giftId}` (כרטיסים זכאים) |
| **Events שמפרסם** | `GiftDrawn { giftId, userId, orderId }` |
| **Events שמאזין** | — |
| **Side effects** | Cart Service מנקה עגלות; Order Service מעדכן `Win=true`; Notification Service שולח מייל |

**לוגיקת הגרלה (כמו היום):**
1. שלוף order IDs למתנה
2. בחר אקראי
3. שמור Winner
4. פרסם `GiftDrawn`
5. Order Service מעדכן `Win=true` (event handler)
6. Cart Service מסיר מתנה מעגלות (event handler)

---

### 3.6 Notification Service (שירות התראות – אופציוני)

**אחריות:** שליחת מייל לזוכה (`EmailService` הקיים)

| פריט | פירוט |
|------|--------|
| **Events שמאזין** | `GiftDrawn`, `UserRegistered` |
| **Controllers נוכחיים** | — (אין API ציבורי) |

---

## 4. ארכיטקטורה מוצעת

```
                         ┌─────────────────┐
                         │   Angular SPA   │
                         └────────┬────────┘
                                  │ HTTPS
                         ┌────────▼────────┐
                         │   API Gateway   │
                         │  (YARP / Ocelot)│
                         └───┬───┬───┬───┬┘
              ┌───────────────┘   │   │   └───────────────┐
              ▼                   ▼   ▼                   ▼
     ┌────────────┐      ┌──────────┐ ┌──────────┐  ┌────────────┐
     │User Service│      │ Catalog  │ │  Cart    │  │   Order    │
     │  :5001     │      │  :5002   │ │  :5003   │  │   :5004    │
     └─────┬──────┘      └────┬─────┘ └────┬─────┘  └─────┬──────┘
           │                  │            │              │
     users_db            catalog_db    cart_db       orders_db
                                                         │
                              ┌──────────────────────────┘
                              ▼
                     ┌────────────────┐
                     │ Raffle Service │
                     │     :5005      │
                     └───────┬────────┘
                             │
                       raffle_db
                              │
                     ┌────────▼────────┐
                     │  Message Broker  │
                     │ (RabbitMQ / ASB) │
                     └────────┬────────┘
                              │
                     ┌────────▼────────┐
                     │ Notification    │
                     │   Service       │
                     └─────────────────┘
```

---

## 5. מיפוי נתונים – מ-SQL לכל שירות

### Monolith (יום)

```
Users ──┬── Bags ──┬── Gifts ── Doners
        │          │     │
        └── Orders ┘     └── Categories
                │
            Winners
```

### מיקרו-סרביסים

| Monolith Table | שירות | שדות Reference |
|----------------|--------|----------------|
| Users | User Service | `_id` / `userId` |
| Gifts, Categories, Doners | Catalog Service | `giftId`, `categoryId`, `donerId` |
| Bags | Cart Service | `userId`, `giftId` (references בלבד) |
| Orders | Order Service | `userId`, `giftId`, `orderGroupId`, `win` |
| Winners | Raffle Service | `userId`, `giftId`, `orderId` |

**אין JOIN בין DB-ים.** במקום:
- `$lookup` / HTTP call בין שירותים
- Eventual consistency
- Denormalization (שמירת `giftName` ב-Order לצורך דוחות)

---

## 6. תקשורת בין שירותים

| תרחיש | סוג | דוגמה |
|-------|-----|-------|
| שליפת מתנה לתצוגה בעגלה | Sync HTTP | Cart → Catalog `GET /gifts/{id}` |
| Checkout | Async Event + Saga | Cart → `CheckoutRequested` → Order |
| הגרלה | Sync + Async | Raffle → Order (HTTP), Raffle → `GiftDrawn` (Event) |
| ניקוי עגלה אחרי הגרלה | Async Event | Cart ← `GiftDrawn` |
| עדכון Win | Async Event | Order ← `GiftDrawn` |

---

## 7. API Gateway – Routing

| Route ציבורי | שירות יעד |
|--------------|-----------|
| `/api/auth/*`, `/api/users/*` | User Service |
| `/api/gifts/*`, `/api/categories/*`, `/api/doners/*` | Catalog Service |
| `/api/bag/*`, `/api/cart/*` | Cart Service |
| `/api/order/*` | Order Service |
| `/api/random/*`, `/api/raffle/*` | Raffle Service |

Gateway גם:
- מאמת JWT (User Service מנפיק)
- מוסיף correlation ID
- Rate limiting

---

## 8. אתגרים ופתרונות

### 8.1 Checkout – Transaction מבוזר

**בעיה:** היום `BagService.ProcessCheckout` עושה הכל ב-transaction אחת.

**פתרון:** Saga (Choreography):
1. Cart שולח `CheckoutRequested`
2. Order יוצר הזמנות
3. Order שולח `OrdersPlaced` / `CheckoutFailed`
4. Cart מנקה רק ב-success

### 8.2 הגרלה – עקביות

**בעיה:** Winner + Order.Win + ניקוי Bags – 3 טבלאות.

**פתרון:** Raffle Service הוא **source of truth** לזוכים. Order מעדכן `Win` דרך event. Cart מנקה דרך event.

### 8.3 מחיקת Gift

**בעיה:** CASCADE ב-SQL מוחק Orders/Bags.

**פתרון:** Soft delete ב-Catalog + event `GiftDeleted` → Cart/Order מטפלים.

### 8.4 דוחות (פופולריות, מחיר)

**בעיה:** Order צריך `Gift.Price` למיון.

**פתרון:** Denormalize `giftPrice`, `giftName` ב-Order בעת יצירה, או קריאה ל-Catalog בזמן query (פחות מומלץ).

---

## 9. שלבי מigration (Strangler Fig)

| שלב | פעולה | סיכון |
|-----|--------|-------|
| 1 | הוצא User Service + JWT | נמוך |
| 2 | הוצא Catalog Service | נמוך |
| 3 | הוצא Cart Service (ללא checkout) | בינוני |
| 4 | הוצא Order Service + Saga | גבוה |
| 5 | הוצא Raffle Service | גבוה |
| 6 | Notification + Gateway | נמוך |

**בכל שלב:** Monolith נשאר fallback עד שהשירות החדש יציב.

---

## 10. MongoDB vs SQL Server per Service

| שירות | DB מומלץ | סיבה |
|-------|----------|------|
| User | SQL / MongoDB | מבנה קבוע, auth |
| Catalog | MongoDB | מסמכים גמישים, embedded category info |
| Cart | Redis + MongoDB | מהיר, TTL אופציונלי |
| Order | SQL / MongoDB | יחסים, דוחות |
| Raffle | MongoDB | מסמכי Winner פשוטים |

---

## 11. סיכום

| מיקרו-סרביס | Bounded Context | Controllers מקור | DB |
|-------------|-----------------|------------------|-----|
| User | זהות והרשאות | Auth, User | users_db |
| Catalog | מתנות וקטalog | Gift, Category, Doner | catalog_db |
| Cart | עגלת קניות | Bag (CRUD) | cart_db |
| Order | הזמנות ו-checkout | Order + checkout | orders_db |
| Raffle | הגרלות וזוכים | Random | raffle_db |
| Notification | מיילים | EmailService | — |

החלוקה משקפת את **תחומי העסק** של Chinese Sale: משתמשים, קטalog מתנות, עגלת קניות, רכישה (כרטיסים), והגרלה – כל אחד עם בעלות נתונים עצמאית ותקשורת מבוקרת.
