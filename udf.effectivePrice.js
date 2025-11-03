function effectivelrice(price, discountPct, taxPct) {
I normalize inputs
var p = Number (price) || 0;
var d = Math. min (Math. max (Number (discountPct) || 0, (
var t = Math.max (Number (taxPct) || 0, O) / 100;
var discounted = p * (1 - d) ;
var taxed = discounted * (1 + t) ;
1/ round to 2 decimals
return Math. round (taxed * 100) / 100;
} 