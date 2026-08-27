const API = `${window.location.origin}/api`;
let currentProduct = null;
let quantity = 1;
let wishlist = JSON.parse(localStorage.getItem("wishlist")) || [];

/* ══════════════════════════════════════════
   INIT
══════════════════════════════════════════ */
window.onload = async () => {
    loadTheme();
    updateCartCount();

    const params = new URLSearchParams(window.location.search);
    const productId = params.get("id");

    if (!productId) {
        showToast("⚠️ No product selected!");
        setTimeout(() => window.location.href = "dashboard.html", 2000);
        return;
    }

    await loadProduct(productId);
    await loadRelated();
};

/* ══════════════════════════════════════════
   LOAD PRODUCT
══════════════════════════════════════════ */
async function loadProduct(id) {
    try {
        const res = await fetch(`${API}/Products/${id}`);
        if (!res.ok) throw new Error("Not found");
        const p = await res.json();
        currentProduct = p;
        renderProduct(p);
    } catch {
        showToast("⚠️ Product not found!");
        setTimeout(() => window.location.href = "dashboard.html", 2000);
    }
}

/* ══════════════════════════════════════════
   RENDER PRODUCT
══════════════════════════════════════════ */
function renderProduct(p) {
    const name = p.name ?? p.Name ?? "Unknown Product";
    const price = Number(p.unitPrice ?? p.price ?? 0);
    const stock = p.stockQuantity ?? p.StockQuantity ?? 0;
    const category = p.categoryName ?? p.category ?? "—";
    const desc = p.description ?? p.Description ?? "No description available.";
    const addedDate = p.addedDate ? new Date(p.addedDate).toLocaleDateString() : "—";
    const productId = p.productId ?? p.id ?? "—";
    const imageUrl = p.imageUrl ?? p.ImageUrl ?? "";

    // Page title & breadcrumb
    document.title = `Electro — ${name}`;
    document.getElementById("breadcrumbName").textContent = name;

    // Main image
    const mainImg = document.getElementById("mainImg");
    mainImg.src = imageUrl || `https://placehold.co/600x600/0e1318/4a5568?text=${encodeURIComponent(name)}`;
    mainImg.alt = name;

    // Thumbnails
    const thumbRow = document.getElementById("thumbRow");
    const thumbImgs = [
        imageUrl || `https://placehold.co/200x200/0e1318/4a5568?text=${encodeURIComponent(name)}`,
        `https://placehold.co/200x200/131a22/4a5568?text=View+2`,
        `https://placehold.co/200x200/0e1318/4a5568?text=View+3`,
        `https://placehold.co/200x200/131a22/4a5568?text=View+4`,
    ];
    thumbRow.innerHTML = thumbImgs.map((src, i) => `
        <div class="thumb ${i === 0 ? 'active' : ''}" onclick="switchImage('${src}', this)">
            <img src="${src}" alt="view ${i + 1}"
                 onerror="this.src='https://placehold.co/200x200/0e1318/4a5568?text=No+Image'"/>
        </div>`).join("");

    // Info
    document.getElementById("productCategory").textContent = category;
    document.getElementById("productName").textContent = name;
    document.getElementById("productPrice").textContent = price.toLocaleString() + " EGP";
    document.getElementById("productDesc").textContent = desc;
    document.getElementById("productStock").textContent = stock > 0 ? `${stock} in stock` : "Out of stock";
    document.getElementById("productCatMeta").textContent = category;
    document.getElementById("productDate").textContent = addedDate;
    document.getElementById("productId").textContent = `#${productId}`;

    // Old price + discount badge
    const oldPrice = Math.round(price * 1.15);
    document.getElementById("productOldPrice").textContent = oldPrice.toLocaleString() + " EGP";
    const disc = document.getElementById("badgeDiscount");
    disc.textContent = "15% OFF";
    disc.style.display = "inline-block";

    // Specs
    document.getElementById("specName").textContent = name;
    document.getElementById("specCategory").textContent = category;
    document.getElementById("specPrice").textContent = price.toLocaleString() + " EGP";
    document.getElementById("specStock").textContent = stock;
    document.getElementById("specDate").textContent = addedDate;
    document.getElementById("specId").textContent = `#${productId}`;

    // Description tab
    document.getElementById("tabDesc").textContent = desc;

    // Out of stock
    if (stock === 0) {
        const btn = document.querySelector(".btn-cart");
        btn.textContent = "Out of Stock";
        btn.disabled = true;
    }

    // Wishlist state
    const isWished = wishlist.includes(productId);
    const wishBtn = document.getElementById("wishBtn");
    if (isWished) {
        wishBtn.textContent = "❤️";
        wishBtn.classList.add("wishlisted");
    }

    // Show NEW badge if added in last 30 days
    if (p.addedDate) {
        const added = new Date(p.addedDate);
        const diff = (new Date() - added) / (1000 * 60 * 60 * 24);
        if (diff <= 30) document.getElementById("badgeNew").style.display = "block";
    }
}

/* ══════════════════════════════════════════
   SWITCH IMAGE (thumbnails)
══════════════════════════════════════════ */
function switchImage(src, el) {
    document.getElementById("mainImg").src = src;
    document.querySelectorAll(".thumb").forEach(t => t.classList.remove("active"));
    el.classList.add("active");
}

/* ══════════════════════════════════════════
   QUANTITY
══════════════════════════════════════════ */
function changeQty(delta) {
    const stock = currentProduct?.stockQuantity ?? currentProduct?.StockQuantity ?? 999;
    quantity = Math.min(Math.max(1, quantity + delta), stock);
    document.getElementById("qtyVal").textContent = quantity;
}

/* ══════════════════════════════════════════
   ADD TO CART
══════════════════════════════════════════ */
function addToCart() {
    if (!currentProduct) return;

    const id = currentProduct.productId ?? currentProduct.id;
    let cart = JSON.parse(localStorage.getItem("cart")) || [];

    for (let i = 0; i < quantity; i++) cart.push(id);
    localStorage.setItem("cart", JSON.stringify(cart));
    updateCartCount();
    showToast(`🛒 "${currentProduct.name ?? "Item"}" × ${quantity} added to cart!`);

    // Button feedback
    const btn = document.querySelector(".btn-cart");
    const original = btn.textContent;
    btn.textContent = "✅ Added!";
    btn.style.opacity = "0.8";
    setTimeout(() => { btn.textContent = original; btn.style.opacity = "1"; }, 2000);
}

/* ══════════════════════════════════════════
   WISHLIST
══════════════════════════════════════════ */
function addToWishlist() {
    if (!currentProduct) return;
    const id = currentProduct.productId ?? currentProduct.id;
    const btn = document.getElementById("wishBtn");
    const idx = wishlist.indexOf(id);

    if (idx === -1) {
        wishlist.push(id);
        btn.textContent = "❤️";
        btn.classList.add("wishlisted");
        showToast(`❤️ "${currentProduct.name}" saved to wishlist!`);
    } else {
        wishlist.splice(idx, 1);
        btn.textContent = "♡";
        btn.classList.remove("wishlisted");
        showToast(`♡ "${currentProduct.name}" removed from wishlist`);
    }
    localStorage.setItem("wishlist", JSON.stringify(wishlist));
}

/* ══════════════════════════════════════════
   LOAD RELATED PRODUCTS
══════════════════════════════════════════ */
async function loadRelated() {
    try {
        const res = await fetch(`${API}/Products`);
        if (!res.ok) throw new Error();
        const data = await res.json();

        const currentId = currentProduct?.productId ?? currentProduct?.id;
        const others = data.filter(p => (p.productId ?? p.id) !== currentId);
        const picked = others.sort(() => Math.random() - 0.5).slice(0, 4);

        document.getElementById("relatedGrid").innerHTML = picked.map(p => {
            const id = p.productId ?? p.id;
            const name = p.name ?? "Product";
            const price = Number(p.unitPrice ?? 0).toLocaleString();
            const img = p.imageUrl ?? p.ImageUrl
                ?? `https://placehold.co/300x300/0e1318/4a5568?text=${encodeURIComponent(name)}`;
            const cat = p.categoryName ?? "";

            return `
            <div class="rel-card" onclick="goToProduct(${id})">
                <img class="rel-img" src="${img}" alt="${name}"
                     onerror="this.src='https://placehold.co/300x300/0e1318/4a5568?text=No+Image'"/>
                <div class="rel-body">
                    <p class="rel-cat">${cat}</p>
                    <p class="rel-name">${name}</p>
                    <p class="rel-price">${price} EGP</p>
                </div>
            </div>`;
        }).join("");

    } catch {
        document.getElementById("relatedGrid").innerHTML =
            `<p style="color:var(--text-muted);grid-column:1/-1">Could not load related products.</p>`;
    }
}

/* ══════════════════════════════════════════
   NAVIGATE TO PRODUCT
══════════════════════════════════════════ */
function goToProduct(id) {
    window.location.href = `product.html?id=${id}`;
}

function goToCart() {
    window.location.href = "cart.html";
}

/* ══════════════════════════════════════════
   TABS
══════════════════════════════════════════ */
function showTab(tabId, el) {
    document.querySelectorAll(".tab-content").forEach(t => t.classList.remove("active"));
    document.querySelectorAll(".tab-btn").forEach(b => b.classList.remove("active"));
    document.getElementById(`tab-${tabId}`).classList.add("active");
    el.classList.add("active");
}

/* ══════════════════════════════════════════
   CART COUNT
══════════════════════════════════════════ */
function updateCartCount() {
    const cart = JSON.parse(localStorage.getItem("cart")) || [];
    const el = document.getElementById("cartCount");
    if (el) el.textContent = cart.length;
}

/* ══════════════════════════════════════════
   TOAST
══════════════════════════════════════════ */
let toastTimer;
function showToast(msg) {
    const t = document.getElementById("toast");
    t.textContent = msg;
    t.classList.add("show");
    clearTimeout(toastTimer);
    toastTimer = setTimeout(() => t.classList.remove("show"), 3000);
}

/* ══════════════════════════════════════════
   THEME
══════════════════════════════════════════ */
function toggleTheme() {
    document.body.classList.toggle("light");
    const isLight = document.body.classList.contains("light");
    document.getElementById("themeBtn").textContent = isLight ? "☀️" : "🌙";
    localStorage.setItem("theme", isLight ? "light" : "dark");
}

function loadTheme() {
    const saved = localStorage.getItem("theme");
    if (saved === "light") {
        document.body.classList.add("light");
        const btn = document.getElementById("themeBtn");
        if (btn) btn.textContent = "☀️";
    }
}

/* ══════════════════════════════════════════
   LOGOUT
══════════════════════════════════════════ */
function logout() {
    localStorage.removeItem("token");
    window.location.href = "../login.html";
}