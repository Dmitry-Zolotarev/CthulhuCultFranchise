Unity C# scripts — минимальный прототип

Содержимое:
- GameManager.cs
- Person.cs
- Room.cs
- OfficeManager.cs
- CityManager.cs
- DayManager.cs
- DragPerson.cs

Это минимальный каркас по предоставленной спецификации.
Значения, которых документ явно не задаёт (например, некоторые величины наград),
в коде являются стартовыми техническими значениями.

Быстрая установка:
1. Создайте Assets/Scripts.
2. Скопируйте туда все .cs.
3. Создайте GameObject с GameManager.
4. Создайте GameObject с OfficeManager и назначьте Person Prefab + Reception Point.
5. Создайте GameObject с CityManager и DayManager.
6. Для Person Prefab добавьте Person, Collider2D и DragPerson.
7. Для комнат добавьте Collider2D + Room и выберите RoomType.
