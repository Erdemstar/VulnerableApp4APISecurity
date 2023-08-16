# VulnerableApp4APISecurity

Merhabalar,

Bu repository OWASP 2019 API'da anlatılan bulguları .NET Core 3.1 API teknolojisi kullanarak geliştirildi. Bu repository hem API güvenliği üzerinde anlatım yaparken örnek bir proje olarak kullanılması hemde şirketlerin API Güvenliğini sağlamak için test ettiği ürünleri denerlerken saldırı senaryosu üretmek için kullanılması amaçlanarak üretilmiştir.

Geliştirilen proje aşağıdaki başıklar açıklanacaktır

- 1- Projenin sahip olduğu zafiyetler
- 1- Projenin ayağı kaldırılması
    - IDE üzerinden ayağı kaldırılması
    - Container üzerinden ayağı kaldırılması
    - Docker-compose üzerinden ayağı kaldırılması
- 1- Proje üzerinde dummy trafiğin oluşturulması
- 1- Projenin sahip olduğu endpointlerin postman çıktısının incelenmesi
- 1- Projenin sahip olduğu enpointleri ve üzerindeki zafiyetlerin ifade edilmesi




### 1- Sahip olunan zafiyetler

- **API 01:2019 — Broken object level authorization**
- **API 02:2019 — Broken authentication**
- **API 03:2019 — Excessive data exposure**
- **API 04:2019 — Lack of resources and rate limiting**
- **API 05:2019 — Broken function level authorization**
- **API 06:2019 — Mass assignment**
- **API 07:2019 — Security misconfiguration**
- **API 08:2019 — Injection**

### 1- Projenin ayağı kaldırılması
İlgil proje .NET Core 3.1 üzerinde oluşturulduğu için işletim sistemi bağımsız her ortamda ayağı kalkabilir. 