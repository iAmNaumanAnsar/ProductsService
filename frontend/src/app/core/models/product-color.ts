export const PRODUCT_COLORS = [
  'Black',
  'White',
  'Red',
  'Green',
  'Blue',
  'Yellow',
  'Silver',
  'Gold',
  'Other'
] as const;

export type ProductColor = (typeof PRODUCT_COLORS)[number];
