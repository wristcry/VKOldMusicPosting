﻿# VKOldMusicPosting

### Что надо чтоб запустить/скомпилить
- .NET Framework 4.8

### Что есть
- Возможность постить пикчи(в виде сетки) с музыкой и текстом
- Возможность делать отложенные посты
- Возможность постить как к себе на страницу, так и в паблик

### Чего нет
- Поддержки двухфакторки
- Встроенной возможности авторизации по своему client_id с client_secret(то что есть я откуда-то нагло украл 5 лет назад)
- Возможности добавлять пикчи из самого ВК

### приколы (вкурсе-мне лень фиксить-и ваще приложение без багов скучная УГАДАЙТЕ)
- В пост нельзя добавить более 10 приложений (т.е. 10 картинок и 1 трек в итоге запостятся как просто 10 картинок) (так работает вк но я мог и учесть кол-во постируемых картинок и музыки)

### ниработаитчтоделать
Запускать с параметром -debug и смотреть почему вк не дает сделать то или иное действие

### FAQ
**Q: Че такое "audio owner id"**
**A:** Айдишник человека/паблика к аудиозаписям которого у страницы с которой вы постите имеется доступ

**Q: Че такое audio id**
**A:** Айдишник трека

**Q: Как получить audio id**
**A:** Аудиозаписи -> Моя музыка(ну или как там) -> ПКМ по треку -> Код элемента -> Пролистать чуть выше до большого блока текста -> data-audio="[audio_id,audio_owner_id]" или data-full-id="audioownerid_audioid"

**Q: Надо ли указывать club/public-/id/тире_перед_цифрами_человека/паблика**
**A:** НЕТ. Разве что на стадии audio owner id - если трек лежит в аудиозаписях паблика - то цифры указывать с тире в начале

**Q: Как залогиниться в проге с другого аккаунта**
**A:** Удалить файл token.owo

**Q: Почему минимум две пикчи**
**A:** Потому что с одной результат будет такой же как при обычном создании поста через сайт(вроде 0_о)

## код - параша

### лан вот вам анек еще
Звонит парень в cекc по телефону:
— Алло, это cекc по телефону?
На другом конце провода:
— Вы не туда попали...
Парень с наслаждением:
— А-а-а-а.... я не туда попал...