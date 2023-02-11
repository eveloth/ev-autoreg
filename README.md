# EV Autoregistrar
EV Autoregistrar — это программа, связывающая корпоративный Exchange сервер и сервисдеск ExtraView, который используется, в частности, в таких компаниях как Pfizer, HP и других.

Этот сервис призван упросить обработку однотипных заявок, поступающих на сервисдеск, позволяя дежурному инженеру сконцентрироваться не на шаблонных действиях, необходимых для регистрации заявок и назначения их на отвественную группу, а на самой работе по заявкам. 

Он позволяет устанавливать правила, по которым будут отбираться заявки для авторегистрации — как по простому вхождению подстрок, так и по регулярным выражениям, а также уточнять делали API-запроса, что актуально в том случае, если конкретная компания настроила сервер ExtraView определённым образом.

### Возможности
- гибкая настройка ролей и разрешений
- неограниченное количество типов заявок
- каскадные обновления — столько запросов к серверу, сколько вам нужно
- вывод логов в реальном времени
- собственный набор правил обработки заявок для каждого пользователя
- динамическая конфигурация — смена Exchange и ExtraView серверов без перезапуска сервиса

### Roadmap

- [ ] композитные правила
- [ ] уведомления
- [ ] UI

### Технологии

.NET 7, ASP.NET Core, PostgreSQL, Redis, gRPC, SignalR Core, Docker, Seq

## Установка

Развёртывание приложения производится в два шага с использованием `docker compose`.

1. Конфигурация

Конфигурация производится при помощи установки переменных окружения через файл `.env`. 

Склонируйте проект и перейдите в его корневую директорию:
```shell
git clone https://github.com/eveloth/ev-autoreg.git
cd ev-autoreg
```
Скопируйте шаблон:
```shell
cp -v .envexample .env
```

Установите следующие минимально необходимые значения:
```env
PG_PASS='' # Пароль от базы данных
SYMMETRICSECURITYKEY='' # 128-ми битный симметричный ключ для шифрования учётных данных
JWT__KEY='' # Ключ подписи JWT токена
REDISOPTIONS__PASSWORD='' # Пароль для Redis
```

Если вы планируете использовать лог-аггрегатор Seq, установите также следующие значения:

```env
SEQOPTIONS__SERVERURL='' # Адрес, по которому доступен Seq
SEQOPTIONS__APIKEY='' # API ключ
```

2. Развёртка

Разверните приложение командой из корневой директории проекта:
```shell
docker compose up -d
```

Учётная запись суперадминистратора создаётся автоматически, учётные данные: `admin@evautoreg.org / P@ssw0rd123`. Рекомендуется сменить их сразу после установки. 
Приложение будет доступно по адресу http://localhost:7444 при использовании настроек по умолчанию. Просматривать логи в реальном времени можно с помощью SignalR клиента по адресу http://localhost:7445/logs.

При развётрке на Windows рекомендуется использовать compose-файл `compose-win.yaml`.

## Принцип работы приложения

Приложение состоит из двух сервисов — REST API для адмнистрирования и управления авторегистратором и самого авторегистратора.

Авторгистратор включает в себя минималистичный почтовый клиент, подключающийся к Exchange серверу, и анализатор заявок. Почтовый клиент анализирует входящие письма на предмет соответсвия их темы регулярному выражению, и в случае совпадения также с помощью регулярного выражения получает из темы письма ID поступившей заявки. После этого заявка загружается в память, сохраняется в базу данных для сбора статистики, а анализатор проверяет, соответствуют ли значения полей заявки правилам, которые указал пользователь. Каждое правило соответствует своему типу заявки, а для каждого типа заявки определены API-запросы к серверу ExtraView, которые выполняются в указанном порядке.
