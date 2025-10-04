# 01-Kavaklıdere - Demo GDD

---

## 1. Giriş

### 1.1 Önsöz
Unity oyun motorunu öğrendiğim ve Unity kullanarak yaptığım **ilk oyunun remake versiyonu**.

Vampire Survivors'tan esinlendiğim bu proje, temel oyun döngüsü, object pooling ve birçok sistem konusunda bilgi edinmemi sağladı. Aynı zamanda yaptığım ilk kendi oyun prototipim olduğu için manevi değeri benim için oldukça yüksek.

**Versiyon Karşılaştırması:** Yeni versiyon tamamlandıktan sonra, eski ve yeni versiyon arasında bulunan 2 yılda kendime neler kattığımı daha net görmek için detaylı bir sürüm farklılıklar dokümanı oluşturacağım.

![Kavaklıdere Old Version](../../Assets/kavaklidere_old_version.png) // Eklenicek
*Orijinal versiyon - 2 yıl önce*

### 1.2 Özet
**Tür:** Bullet Heaven / Rogue-like Action

**Ana Hedef:** Vampire Survivors'ta olduğu gibi rogue-like yapıya sahip olan projede, düşmanları öldürerek stage-by-stage ilerleme. Her stage sonunda oyun zorlaşır ve level-up'lar ile güçlenme.

**Ana Fark:** Oto saldırı yerine **20 Minutes Till Dawn**'dan esinlenilen daha aktif bir kombat sistemi. Ufak fizik etkileşimleri de eklenecek.

![20 Minutes Till Dawn Reference](../../Assets/20_Minutes_Til_Dawn.jpg)
*20 Minutes Till Dawn - Aktif kombat sistemi referansı*

![Vampire Survivors Reference](../../Assets/Vampire_Survivors.jpg)
*Vampire Survivors - Rogue-like progression referansı*

---

## 2. Oynanış

### 2.1 Düşman Tiplemeleri

Oyunda **3 farklı düşman tiplemesi** bulunacak:

#### Melee Düşmanlar
**Rol:** Yakın mesafe saldırıları yapan, oyuncu temposunu dengelemek amaçlı düşmanlar
- Orta seviye can
- Yavaş hareket
- Yüksek yakın mesafe hasarı
- Oyuncuyu kovalama davranışı

#### Ranged Düşmanlar  
**Rol:** Uzak mesafeden oyuncuyu ara ara tetiklemek amaçlı düşük cana sahip düşmanlar
- Düşük can
- Mesafeyi koruma davranışı
- Periyodik ateş etme
- Oyuncuya doğrudan koşmaz

#### Dasher Düşmanlar
**Rol:** Oyuncuya doğru dash atarak çeşitli fizik etkileşimlerine sokan yüksek canlı düşmanlar
- Yüksek can
- Dash attack mekaniği
- Knockback efekti
- Cooldown sonrası tekrar dash
**Düşman Denge Tablosu:**

| Düşman Tipi | Can | Hasar | Hız | Özel Özellik |
|-------------|-----|-------|-----|--------------|
| **Melee** | Orta | Yüksek | Yavaş | Yakın mesafe odaklı |
| **Ranged** | Düşük | Orta | Orta | Mesafe tutar |
| **Dasher** | Yüksek | Yüksek | Hızlı (Dash) | Fizik etkileşimi |

### 2.2 Oyuncu Hareketi

#### Temel Hareket
Klasikleşmiş **2D top-down hareket sistemi** ile oyuncu kontrolü.

#### Dash Mekaniği
**Özellikler:**
- Belirli cooldown süresi
- Dash sırasında **hasar alınamaz**
- Objelerden **geçilebilir**
- **Risk-Reward:** Obje içinde kalınırsa saniye başına hasar

**Stratejik Kullanım:**
- Gerekli durumlarda binaların içine saklanma
- Düşman kalabalığından kaçış
- Tuzaklı alanlardan geçiş
- Yiyeceği hasarı kabul ederek güvenli pozisyon alma

### 2.3 Kombat ve Upgrade Sistemi

#### Temel Upgrade Parametreleri
Başlangıçta 5 farklı upgrade seçeneği:

**1. Can Arttırma**
- Max canı arttırır
- **Önemli:** Upgrade alınırken iyileşme olmaz
- Stratejik karar gerektir

**2. Saldırı Hızı**
- Saniye başına atılan mermi sayısını arttırır
- DPS (Damage Per Second) artar
- Daha yoğun ateş gücü

**3. İyileşme**
- Mevcut canı tam cana çıkartır
- Tek seferlik etki
- **Stratejik Seçim:** Can arttırma vs İyileşme

**4. Mermi Hızı**
- Mermilerin hızını arttırır
- **Fizik Etkileşimi:** Düşmanlarda geri tepme (knockback) yaratır
- Daha hızlı mermi = Daha fazla knockback
- Crowd control için kullanılabilir

**5. Dash Cooldown**
- Dash bekleme süresini azaltır
- Daha sık dash atabilme
- Survival için kritik parametre
- Dash ile hasar alınmadığı için güvenli geçişler
 ---
## 3. Mekanikler

### 3.1 Fizik Etkileşimleri

**Knockback Sistemi:**
- Mermi hızı → Düşman geri tepmesi
- Dasher düşmanlar → Oyuncu geri tepmesi
- Çevresel objelerle çarpışma
- Zincirleme itme efektleri
- Oyuncu silahı ile çarpışma ve hasar yaratabilir

### 3.2 Stage Progression

**Stage Yapısı:**
- Her stage artan zorluk
- Daha fazla düşman spawn
- Daha güçlü düşman kombinasyonları
- Boss encounter'lar (planlanan)

### 3.3 Object Pooling Sistemi

Performans optimizasyonu için kritik:
- Düşman pooling
- Mermi pooling
- Audio pooling

---

## 4. Teknik Hedefler

### 4.1 Versiyon Karşılaştırması (Planlanan)

**Eski Versiyon (2022):**
- Basit hareket sistemi
- Tek düşman hareketi 
- Performans sorunları

**Yeni Versiyon (2024):**
- Gelişmiş kombat
- 3 düşman tipi
- Object pooling
- Optimize edilmiş sistemler
---

> **Hazırlayan:** Ethem Emre Özkan  
> **Tarih:** 03.10.025
> **Proje Durumu:** 🚧 Aktif Geliştirme

